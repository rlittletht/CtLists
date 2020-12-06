using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCore;
using TCore.KeyVault;

namespace CtLists
{
    public class CtSql
    {
        private string s_sAppID = "bfbaffd7-2217-4deb-a85a-4f697e6bdf94";
        private string m_sAppTenant = "b90f9ef3-5e11-43e0-a75c-1f45e6b223fb";
        private string s_sConnectionStringSecretID = "Thetasoft-Azure-ConnectionString/324deaac388a480ab992ccef03072b61";

        private Client m_clientKeyVault = null;
        string sSqlConnectionString = null;


        public CtSql()
        {
        }

        async Task EnsureSqlConnectionString()
        {
            if (m_clientKeyVault == null)
                m_clientKeyVault = new Client(m_sAppTenant, s_sAppID);

            if (sSqlConnectionString == null)
                sSqlConnectionString = await m_clientKeyVault.GetSecret(s_sConnectionStringSecretID);
        }

        void FixLeadingZeros(Sql sql, HashSet<string> hashBottlesWithMissingLeadingZero1, HashSet<string> hashBottlesWithMissingLeadingZero2)
        {
            sql.BeginTransaction();
            foreach (string s in hashBottlesWithMissingLeadingZero1)
            {
                sql.SExecuteScalar($"update upc_wines set ScanCode='{s}' where ScanCode='{s.Substring(1)}'");
                sql.SExecuteScalar($"update upc_codes set ScanCode='{s}' where ScanCode='{s.Substring(1)}'");
            }

            foreach (string s in hashBottlesWithMissingLeadingZero2)
            {
                sql.SExecuteScalar($"update upc_wines set ScanCode='{s}' where ScanCode='{s.Substring(2)}'");
                sql.SExecuteScalar($"update upc_codes set ScanCode='{s}' where ScanCode='{s.Substring(2)}'");
            }

            sql.Commit();

        }

        public async Task<(int, int, int, int)> UpdateLocalDatabaseFromDownloadedCellar(Cellar cellar, bool fFixLeadingZeros, bool fPreflightOnly)
        { 
            await EnsureSqlConnectionString();

            // loop over all of our bottles and add the ones that are missing, and/or fix the scancodes for those
            // that have missing leading zeros
            SR sr;

            sr = TCore.Sql.OpenConnection(out Sql sql, sSqlConnectionString);

            if (!sr.Succeeded)
                throw new Exception($"can't open SQL connection: {sr.Reason}");

            string sSelect = "select ScanCode from upc_wines";

            sql.ExecuteReader(sSelect, out SqlReader sqlr, null);
            HashSet<string> hashOurBottles = new HashSet<string>();

            while (sqlr.Reader.Read())
            {
                string s = sqlr.Reader.GetString(0);
                hashOurBottles.Add(s);
            }

            sqlr.Close();

            // now we have all of the bottles we know about
            // build all the bottles from cellartracker

            HashSet<string> hashTheirBottles = new HashSet<string>();
            foreach (Bottle bottle in cellar.Bottles)
            {
                hashTheirBottles.Add(bottle.Barcode);
            }

            // now, which bottles do they have, but we don't
            HashSet<string> hashBottlesOnlyInCellarTracker = new HashSet<string>();
            HashSet<string> hashBottlesWithMissingLeadingZero = new HashSet<string>();
            HashSet<string> hashBottlesWithMissingLeadingZero2 = new HashSet<string>();
            HashSet<string> hashBottlesWithMissingLeadingZero3 = new HashSet<string>();
            HashSet<string> hashBottlesOnlyInOurCellar = new HashSet<string>();

            foreach (string s in hashTheirBottles)
            {
                if (!hashOurBottles.Contains(s))
                {
                    // bottle is missing. check to see if there's a missing leading zero
                    if (s.StartsWith("0") && hashOurBottles.Contains(s.Substring(1)))
                    {
                        hashBottlesWithMissingLeadingZero.Add(s);
                    }
                    else if (s.StartsWith("00") && hashOurBottles.Contains(s.Substring(2)))
                    {
                        hashBottlesWithMissingLeadingZero2.Add(s);
                    }
                    else if (s.StartsWith("000") && hashOurBottles.Contains(s.Substring(3)))
                    {
                        hashBottlesWithMissingLeadingZero3.Add(s);
                    }

                    else
                    {
                        hashBottlesOnlyInCellarTracker.Add(s);
                    }
                }
            }

            // now, let's not have the jurassic park problem...check the other direction too
            foreach (string s in hashOurBottles)
            {
                if (!hashTheirBottles.Contains(s))
                {
                    // bottle is not on cellar tracker...check to see if missing leading zero
                    if (hashTheirBottles.Contains($"0{s}"))
                    {
                        if (!hashBottlesWithMissingLeadingZero.Contains($"0{s}"))
                        {
                            MessageBox.Show($"Strange. We had a missing leading zero, didn't find it in the first pass... ({s})");
                            hashBottlesWithMissingLeadingZero.Add($"0{s}");
                        }
                    }
                    else
                    {
                        // CT only tells us about undrunk wines...so maybe this is one we already drank?
                        // or its one that we knew about at one point, but not later...
                        // let's not worry about them
                        // hashBottlesOnlyInOurCellar.Add(s);
                    }
                }
            }

            if (!fPreflightOnly)
            {
                // at this point, we know what we have to add to our cellar, and what we have to fix
                MessageBox.Show(
                    $"BrokenZeros1: {hashBottlesWithMissingLeadingZero.Count}, BrokenZeros2: {hashBottlesWithMissingLeadingZero2.Count}, BrokenZeros3: {hashBottlesWithMissingLeadingZero3.Count}. {hashBottlesOnlyInOurCellar.Count} only in our cellar, and {hashBottlesOnlyInCellarTracker.Count} only in CellarTracker");


                if (fFixLeadingZeros && (hashBottlesWithMissingLeadingZero.Count > 0 || hashBottlesWithMissingLeadingZero2.Count > 0))
                {
                    FixLeadingZeros(sql, hashBottlesWithMissingLeadingZero, hashBottlesWithMissingLeadingZero2);
                }

                // now add any bottles that haven't been added yet...
                //
                sql.BeginTransaction();

                foreach (string s in hashBottlesOnlyInCellarTracker)
                {
                    Bottle bottle = cellar[s];

                    string sInsert =
                        "insert into upc_wines (ScanCode, Wine, Vintage, Locale, Country, Region, SubRegion, Appelation, Producer, [Type], Color, Category, Varietal, Designation, Vineyard, Score, [Begin], [End], iWine, Consumed, Notes, UpdatedCT, Bin, Location)"
                        + " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}',{21},'{22}','{23}')";

                    string sConsumed = bottle.GetValueOrEmpty("Consumed");

                    if (sConsumed.Length == 0)
                        sConsumed = "1900-01-01 00";

                    string sUpdatedCT = bottle.GetValueOrEmpty("UpdatedCT");

                    if (sUpdatedCT.Length == 0)
                        sUpdatedCT = "0";

                    string sQuery = String.Format(
                        sInsert,
                        Sql.Sqlify(bottle.GetValueOrEmpty("Barcode")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Wine")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Vintage")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Locale")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Country")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Region")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("SubRegion")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Appellation")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Producer")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Type")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Color")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Category")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Varietal")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Designation")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Vineyard")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Score")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Begin")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("End")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("iWine")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Consumed")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Notes")),
                        Sql.Sqlify(sUpdatedCT),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Bin")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Location")));

                    string sResult = sql.SExecuteScalar(sQuery);

                    sQuery = String.Format(
                        "insert into upc_codes (ScanCode, DescriptionShort, FirstScanDate,LastScanDate) VALUES ('{0}','{1}','{2}','{3}')",
                        Sql.Sqlify(bottle.GetValueOrEmpty("Barcode")),
                        Sql.Sqlify(bottle.GetValueOrEmpty("Wine")),
                        "1900-01-01 00:00:00.000",
                        "1900-01-01 00:00:00.000");

                    sResult = sql.SExecuteScalar(sQuery);
                }
                sql.Commit();
            }
            return (hashBottlesWithMissingLeadingZero.Count, hashBottlesWithMissingLeadingZero2.Count, hashBottlesOnlyInOurCellar.Count, hashBottlesOnlyInCellarTracker.Count);


        }

        static string s_sSqlBottleQuery = "SELECT ScanCode, Wine, Vintage, Locale, Country, Region, SubRegion, Appelation, Producer, [Type], Color, Category, Varietal, Designation, Vineyard, Score, [Begin], [End], iWine, Notes, Bin, Location, UpdatedCT, Consumed "
                                          + "FROM upc_wines";
        static string s_sSqlDrunkWinesWhere = " WHERE DatePart(year, IsNull(Consumed, '1900-01-01 00:00:00.000')) <> 1900";
        static string s_sSqlBinnedWinesWhere = " WHERE Bin<>''"; // And DatePart(year, IsNull(Consumed, '1900-01-01 00:00:00.000')) = 1900";

        void SetValueFromSqlString(Bottle bottle, SqlReader sqlr, int i, string sKey)
        {
            if (!sqlr.Reader.IsDBNull(i))
                bottle.SetValue(sKey, sqlr.Reader.GetString(i));
        }

        Bottle BottleFromReader(SqlReader sqlr)
        {
            Bottle bottle = new Bottle();

            int i = 0;

            SetValueFromSqlString(bottle, sqlr, i++, "Barcode");
            SetValueFromSqlString(bottle, sqlr, i++, "Wine");
            SetValueFromSqlString(bottle, sqlr, i++, "Vintage");
            SetValueFromSqlString(bottle, sqlr, i++, "Locale");
            SetValueFromSqlString(bottle, sqlr, i++, "Country");
            SetValueFromSqlString(bottle, sqlr, i++, "Region");
            SetValueFromSqlString(bottle, sqlr, i++, "SubRegion");
            SetValueFromSqlString(bottle, sqlr, i++, "Appellation");
            SetValueFromSqlString(bottle, sqlr, i++, "Producer");
            SetValueFromSqlString(bottle, sqlr, i++, "Type");
            SetValueFromSqlString(bottle, sqlr, i++, "Color");
            SetValueFromSqlString(bottle, sqlr, i++, "Category");
            SetValueFromSqlString(bottle, sqlr, i++, "Varietal");
            SetValueFromSqlString(bottle, sqlr, i++, "Designation");
            SetValueFromSqlString(bottle, sqlr, i++, "Vineyard");
            SetValueFromSqlString(bottle, sqlr, i++, "Score");
            SetValueFromSqlString(bottle, sqlr, i++, "Begin");
            SetValueFromSqlString(bottle, sqlr, i++, "End");
            SetValueFromSqlString(bottle, sqlr, i++, "iWine");
            SetValueFromSqlString(bottle, sqlr, i++, "Notes");
            SetValueFromSqlString(bottle, sqlr, i++, "Bin");
            SetValueFromSqlString(bottle, sqlr, i++, "Location");
            bottle.SetValue("UpdatedCT", sqlr.Reader.GetBoolean(i++).ToString());
            // consumed is a date...
            if (!sqlr.Reader.IsDBNull(i))
                bottle.SetValue("Consumed", sqlr.Reader.GetDateTime(i).ToString("MM/dd/yyyy"));
            i++;


            return bottle;
        }


        public async Task<Dictionary<string, Bottle>> GetBottlesToDrink()
        {
            await EnsureSqlConnectionString();

            // get all of the consumed bottles we know about
            SR sr;

            sr = TCore.Sql.OpenConnection(out Sql sql, sSqlConnectionString);

            if (!sr.Succeeded)
                throw new Exception($"can't open SQL connection: {sr.Reason}");

            string sQuery = $"{s_sSqlBottleQuery} {s_sSqlDrunkWinesWhere}";
            sql.ExecuteReader(sQuery, out SqlReader sqlr, null);

            Dictionary<string, Bottle> bottles = new Dictionary<string, Bottle>();
            while (sqlr.Reader.Read())
            {
                Bottle bottle = BottleFromReader(sqlr);

                bottles.Add(bottle.Barcode, bottle);
            }

            sqlr.Close();

            return bottles;
        }

        public async Task<Dictionary<string, Bottle>> GetBottlesToRelocate()
        {
            await EnsureSqlConnectionString();

            // get all of the consumed bottles we know about
            SR sr;

            sr = TCore.Sql.OpenConnection(out Sql sql, sSqlConnectionString);

            if (!sr.Succeeded)
                throw new Exception($"can't open SQL connection: {sr.Reason}");

            string sQuery = $"{s_sSqlBottleQuery} {s_sSqlBinnedWinesWhere}";
            sql.ExecuteReader(sQuery, out SqlReader sqlr, null);

            Dictionary<string, Bottle> bottles = new Dictionary<string, Bottle>();
            while (sqlr.Reader.Read())
            {
                Bottle bottle = BottleFromReader(sqlr);
                bottles.Add(bottle.Barcode, bottle);
            }

            sqlr.Close();

            return bottles;
        }
    }
}