using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;
using RDotNet;
using System.Text.RegularExpressions;
using System.Collections;
using System.Configuration;

namespace DatabaseMigratior
{
    public partial class Form1 : Form
    {
        string directoryForDialog = "c:\\";
        enum CRNcolumIndex
        {
            SITENAME = 0,
            CDATE = 1,
            T_DAILY_MIN = 6,
            T_DAILY_MAX = 5,
            T_DAILY_MEAN = 7,
            T_DAILY_AVG = 8,
            PPT_DAILY = 9,
            RH_DAILY_MIN = 16,
            RH_DAILY_MAX = 15,
            RH_DAILY_MEAN = -1,
            VPD_DAILY_AVG = -1,
            AGDD = -1,
            DAY_LENGTH = -1,
            SOIL_MOISTURE_5_DAILY = 18,
            SOIL_MOISTURE_10_DAILY = 19,
            SOIL_MOISTURE_20_DAILY = 20,
            SOIL_MOISTURE_50_DAILY = 21,
            SOIL_MOISTURE_100_DAILY = 22,
           
            SOIL_TEMP_5_DAILY = 23,
            SOIL_TEMP_10_DAILY = 24,
            SOIL_TEMP_20_DAILY = 25,
            SOIL_TEMP_50_DAILY = 26,
            SOIL_TEMP_100_DAILY = 27

            }
        enum columnIndex
        {
            SITENAME=0,
            CDATE=1,
            T_DAILY_MIN=6,
            T_DAILY_MAX=5,
            T_DAILY_MEAN=-1,
            T_DAILY_AVG=7,
            PPT_DAILY=8,
            RH_DAILY_MIN=94,
            RH_DAILY_MAX=95,
            RH_DAILY_MEAN=-1,
            VPD_DAILY_AVG=-1,
            AGDD=-1,
            DAY_LENGTH=-1,
            SOIL_MOISTURE_5_DAILY_DUNE=9,
            SOIL_MOISTURE_10_DAILY_DUNE=10,
            SOIL_MOISTURE_20_DAILY_DUNE=11,
            SOIL_MOISTURE_50_DAILY_DUNE=12,
            SOIL_MOISTURE_100_DAILY_DUNE=13,
            SOIL_MOISTURE_5_DAILY_GRASS=14,
            SOIL_MOISTURE_10_DAILY_GRASS=15,
            SOIL_MOISTURE_20_DAILY_GRASS=16,
            SOIL_MOISTURE_50_DAILY_GRASS=17,
            SOIL_MOISTURE_100_DAILY_GRASS=18,
            SOIL_MOISTURE_5_DAILY_BARE=19,
            SOIL_MOISTURE_10_DAILY_BARE=20,
            SOIL_MOISTURE_20_DAILY_BARE=21,
            SOIL_MOISTURE_50_DAILY_BARE=22,
            SOIL_MOISTURE_100_DAILY_BARE=23,
            SOIL_TEMP_5_DAILY_DUNE=39,
            SOIL_TEMP_10_DAILY_DUNE=40,
            SOIL_TEMP_20_DAILY_DUNE=41,
            SOIL_TEMP_50_DAILY_DUNE=42,
            SOIL_TEMP_100_DAILY_DUNE=43,
            SOIL_TEMP_5_DAILY_GRASS=44,
            SOIL_TEMP_10_DAILY_GRASS=45,
            SOIL_TEMP_20_DAILY_GRASS=46,
            SOIL_TEMP_50_DAILY_GRASS=47,
            SOIL_TEMP_100_DAILY_GRASS=48,
            SOIL_TEMP_5_DAILY_BARE=49,
            SOIL_TEMP_10_DAILY_BARE=50,
            SOIL_TEMP_20_DAILY_BARE=51,
            SOIL_TEMP_50_DAILY_BARE=52,
            SOIL_TEMP_100_DAILY_BARE=53,
        }
        //string connString = "server=localhost; user id=root; password= Jrn.phenomet;database=phenomet; pooling=false; Persist Security Info=false;";
        string connString = "";
        string replaceOrignore = "INSERT IGNORE";
        string S_replaceOrignore = "INSERT IGNORE";

        private void GetConnectionStringByName()
        {
            // Look for the name in the connectionStrings section.
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["phenologyDBConnection"]; //phenologyLocalDBConnection

            // If found, return the connection string. 
            if (settings != null)
                connString = settings.ConnectionString;
        }

        private double siteNametoLatitude(string sitename)
        {
            if (sitename == "pas9")
            {
                return 32.57522;
            }
            else if (sitename == "gibpe")
            {
                return 32.589096;
            }
            else if (sitename == "tweedie")
            {
                return 32.582011;
            }
            else if (sitename == "tromble")
            {
                return 32.585119;
            }
            else if (sitename == "crn")
            {
                return 32.613655;
            }
            else
            {
                return 32.556582;
            }
        }
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setRDotNotDll();
            GetConnectionStringByName();

            for (int i = 1; i <= 31; i++)
            {
                cmbDay1.Items.Add(i);
                cmbDay2.Items.Add(i);
                cmbDay3.Items.Add(i);
                cmbDay4.Items.Add(i);
            }

            System.Globalization.DateTimeFormatInfo info = System.Globalization.DateTimeFormatInfo.GetInstance(null);
            for (int i = 1; i < 13; i++)
            {
                cmbMonth1.Items.Add(info.GetMonthName(i));
                cmbMonth2.Items.Add(info.GetMonthName(i));
                cmbMonth3.Items.Add(info.GetMonthName(i));
                cmbMonth4.Items.Add(info.GetMonthName(i));

            }

        }

        private void setRDotNotDll()
        {
            string rpath = ConfigurationManager.AppSettings["Rpath"];

            var oldPath = System.Environment.GetEnvironmentVariable("PATH");
            //var rPath = @"C:\Program Files\R\R-3.1.0\bin\i386";
            var rPath = rpath;
            var newPath = string.Format("{0}{1}{2}", rPath, System.IO.Path.PathSeparator, oldPath);

            System.Environment.SetEnvironmentVariable("PATH", newPath);
        }

        private double dayLengthCalculator(DateTime d, string siteName)
        {
            //REngine.SetDllDirectory(@"C:\Program Files\R\R-3.1.0\bin\i386");
            
            REngine engine;
            if (REngine.GetInstanceFromID("RDontNet") == null)
            {
                engine = REngine.CreateInstance("RDontNet");
                engine.Initialize();
            }
            else
            {
                engine = REngine.GetInstanceFromID("RDontNet");
            }
            string dayLengthCal=@"daylength <- function(lat, doy) {
	                            if (class(doy) == 'Date' | class(doy) == 'character') { 
		                            doy <- doyFromDate(doy) 
	                            }
	                            lat[lat > 90 | lat < -90] <- NA 
	                            doy[doy==366] <- 365
	                            doy[doy < 1] <- 365 + doy[doy < 1]
	                            doy[doy > 365] <- doy[doy > 365] - 365 
	                            if (isTRUE(any(doy<1))  | isTRUE(any(doy>365))) {
		                            stop('cannot understand value for doy') 
	                            }
	                            P <- asin(0.39795 * cos(0.2163108 + 2 * atan(0.9671396 * tan(0.00860*(doy-186)))))
	                            a <-  (sin(0.8333 * pi/180) + sin(lat * pi/180) * sin(P)) / (cos(lat * pi/180) * cos(P));
	                            a <- pmin(pmax(a, -1), 1)
	                            DL <- 24 - (24/pi) * acos(a)
	                            return(DL)
                            }";
            Function daylength = engine.Evaluate(dayLengthCal).AsFunction();
            double l = this.siteNametoLatitude(siteName);
            //NumericVector par = engine.CreateNumericVector(new double[] { l, d.DayOfYear});
            NumericVector doy = engine.Evaluate(@"doy<-" + d.DayOfYear).AsNumeric();
            NumericVector lat = engine.Evaluate(@"lat<-" + l).AsNumeric();
            NumericVector dayl = engine.Evaluate(@"dayl <- daylength(lat,doy)").AsNumeric();
            double day_len = dayl[0];
            //engine.Dispose();
            return day_len;
        }

        private void openDialog(string txtButton)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = directoryForDialog;
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            directoryForDialog = openFileDialog1.FileName;
                            if (txtButton == "solarNoon")
                            {
                                textBox2.Text = openFileDialog1.FileName;
                            }
                            else
                            {
                                textBox1.Text = openFileDialog1.FileName;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openDialog("climate");
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            this.openDialog("climate");
        }
        private bool checkExistScan(string[] values, MySqlConnection conn)
        {
            string[] columnNames ={
                                        "DATE",
                                        "SITE_CODE",
                                        "T_DAILY_MIN",
                                        "T_DAILY_MAX",
                                        "T_DAILY_MEAN",
                                        "T_DAILY_AVG",
                                        "PPT_DAILY_MM",
                                        "RH_DAILY_MIN",
                                        "RH_DAILY_MAX",
                                        "RH_DAILY_MEAN",
                                        "VPD_DAILY_AVG",
                                        "AGDD",
                                        "DAY_LENGTH",
                                        "SOIL_MOISTURE_5_DAILY_DUNE",
                                        "SOIL_MOISTURE_10_DAILY_DUNE",
                                        "SOIL_MOISTURE_20_DAILY_DUNE",
                                        "SOIL_MOISTURE_50_DAILY_DUNE",
                                        "SOIL_MOISTURE_100_DAILY_DUNE",
                                        "SOIL_MOISTURE_5_DAILY_GRASS",
                                        "SOIL_MOISTURE_10_DAILY_GRASS",
                                        "SOIL_MOISTURE_20_DAILY_GRASS",
                                        "SOIL_MOISTURE_50_DAILY_GRASS",
                                        "SOIL_MOISTURE_100_DAILY_GRASS",
                                        "SOIL_MOISTURE_5_DAILY_BARE",
                                        "SOIL_MOISTURE_10_DAILY_BARE",
                                        "SOIL_MOISTURE_20_DAILY_BARE",
                                        "SOIL_MOISTURE_50_DAILY_BARE",
                                        "SOIL_MOISTURE_100_DAILY_BARE",
                                        "SOIL_TEMP_5_DAILY_DUNE",
                                        "SOIL_TEMP_10_DAILY_DUNE",
                                        "SOIL_TEMP_20_DAILY_DUNE",
                                        "SOIL_TEMP_50_DAILY_DUNE",
                                        "SOIL_TEMP_100_DAILY_DUNE",
                                        "SOIL_TEMP_5_DAILY_GRASS",
                                        "SOIL_TEMP_10_DAILY_GRASS",
                                        "SOIL_TEMP_20_DAILY_GRASS",
                                        "SOIL_TEMP_50_DAILY_GRASS",
                                        "SOIL_TEMP_100_DAILY_GRASS",
                                        "SOIL_TEMP_5_DAILY_BARE",
                                        "SOIL_TEMP_10_DAILY_BARE",
                                        "SOIL_TEMP_20_DAILY_BARE",
                                        "SOIL_TEMP_50_DAILY_BARE",
                                        "SOIL_TEMP_100_DAILY_BARE",
                                  };

            string sql = "SELECT * FROM sc_std_clim WHERE ";
            for (int i = 0; i < columnNames.Length; i++)
            {
                if (i < columnNames.Length - 1)
                {
                    sql += columnNames[i] + "=" + values[i] + " AND ";
                }
                else
                {
                    sql += columnNames[i] + "=" + values[i] + ";";
                }
            }
            MySqlCommand check = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = check.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Close();
                return true;
            }
            else
            {
                rdr.Close();
                return false;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Raw File has not been chosen!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (comboBox1.SelectedItem==null)
            {
                MessageBox.Show("Site has not been chosen!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                directoryForDialog = textBox1.Text;
                string site = comboBox1.SelectedItem.ToString();
                switch (site)
                {
                    case "SCAN":
            
                        if (Path.GetExtension(textBox1.Text) == ".csv")
                        {
                            this.lockUnlockGUI("Lock");
                            this.ImportSCAN(textBox1.Text);
                            this.lockUnlockGUI("Unlock");
                        }
                        else
                        {
                            MessageBox.Show("File has to be in .csv format", "File Type Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case "GIBPE":

                        if (Path.GetExtension(textBox1.Text) == ".dat" || Path.GetExtension(textBox1.Text) == ".csv")
                        {
                            this.lockUnlockGUI("Lock");
                            this.ImportGIBPE(textBox1.Text);
                            this.lockUnlockGUI("UnLock");
                        }
                        else
                        {
                            MessageBox.Show("File has to be in .dat format", "File Type Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case "CRN":
                        if (Path.GetExtension(textBox1.Text) == ".txt")
                        {
                            this.lockUnlockGUI("Lock");
                            this.ImportCRN(textBox1.Text);
                            this.lockUnlockGUI("Unlock");
                        }
                        else
                        {
                            MessageBox.Show("File has to be in .txt format", "File Type Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case "TROM":
                        if (Path.GetExtension(textBox1.Text) == ".csv")
                        {
                            this.lockUnlockGUI("Lock");
                            this.ImportTROM(textBox1.Text);
                            this.lockUnlockGUI("Unlock");
                        }
                        else
                        {
                            MessageBox.Show("File has to be in .csv format", "File Type Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case "TWEE":
                        if (Path.GetExtension(textBox1.Text) == ".csv")
                        {
                            this.lockUnlockGUI("Lock");
                            this.ImportTWE(textBox1.Text);
                            this.lockUnlockGUI("Unlock");
                        }
                        else
                        {
                            MessageBox.Show("File has to be in .csv format", "File Type Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case "PAS9":
                        if (Path.GetExtension(textBox1.Text) == ".dat" || Path.GetExtension(textBox1.Text) == ".csv")
                        {
                            this.lockUnlockGUI("Lock");
                            this.ImportPAS9(textBox1.Text);
                            this.lockUnlockGUI("Unlock");
                        }
                        else
                        {
                            MessageBox.Show("File has to be in either .csv or .dat format", "File Type Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    default:
                        MessageBox.Show("You Must Choose File Type!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }

        }
        
        /*My Implementation*/
        private void ImportPAS9(string fileAddress)
        {
            int i = 0, k = 0, db_rows_count = 0;
            bool exists = false;
            DateTime date;

            MySqlConnection conn = null;
            MySqlCommand cmd = null;

            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //-----------------PROGRESS BAR setup---------------------------------
            var lines = File.ReadLines(fileAddress);
            int TotalLines = lines.Count();
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = TotalLines;
            //---------------------------------------------------------

            string data = "", sql = "";

            if (radioButton6.Checked == true) //in case of dailyTemp file
            {
                double[] datavalues = new double[6];
                db_rows_count = 0;
                foreach (string line in File.ReadLines(@fileAddress))
                {
                    i++;
                    if (i == 1 || i == 2 || i == 3 || i == 4) continue;
                   
                    data = line;
                    data = data.Replace("\"", "");
                    data = data.Replace(" ", "");
                    data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                    data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                    string[] values = data.Split(',');

                    if (values.Length == 1)
                    {
                        continue;
                    }
                    if (values.Length != 25)
                    {
                        MessageBox.Show("PAS9 raw file is not in correct format. " + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    date = DateTime.ParseExact(values[0], "yyyy-MM-ddHH:mm:ss", null);
                    date = new DateTime(date.Year, date.Month, date.Day); //removing minutes

                    //TempDailyMin
                    datavalues[0] = double.Parse(values[23]);
                    //TempDailyMax
                    datavalues[1] = double.Parse(values[24]);
                    ////TempDailyMean
                    datavalues[2] = (datavalues[0] + datavalues[1]) / 2;
                    //TempDailyAvg
                    datavalues[3] = double.Parse(values[22]);

                    //AGDD
                    datavalues[4] = -99.99;
                    //Day Length
                    datavalues[5] = this.dayLengthCalculator(date, "pas9");

                    bool inserted_tempdata = false;
                    exists = checkIfDateExists(date, "p9_std_clim", "inserted_temp_data", out inserted_tempdata, conn);

                    k++;
                    toolStripProgressBar1.PerformStep();

                    if (inserted_tempdata && radioButton1.Checked)
                        continue;
                    //exists = checkExistData(date, "p9_std_clim", conn);
                    sql = create_sql_of_StdP9(date, datavalues, exists, "P9_T",inserted_tempdata);

                    //-----------------------------end------------------------------------------------
                    try
                    {
                        cmd = new MySqlCommand(sql, conn);
                        int insertOrUpdate = cmd.ExecuteNonQuery();
                        db_rows_count = insertOrUpdate > 1 ? db_rows_count + 1 : db_rows_count + insertOrUpdate;

                    }
                    catch (MySqlException e)
                    {
                        MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    
                }//end of while

                MessageBox.Show(k + " records have been read from the raw file. " + db_rows_count + " rows have been effected in the database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }//end of radiobutton6
            else if (radioButton3.Checked == true) //in case of dailyPPT file
            {
                double[] PPT_daily = new double[1];
                foreach (string line in File.ReadLines(@fileAddress))
                {
                    i++;
                    if (i == 1 || i == 2 || i == 3 || i == 4) continue;

                    data = line;
                    data = data.Replace("\"", "");
                    data = data.Replace(" ", "");
                    data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                    data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                    string[] values = data.Split(',');

                    if (values.Length == 1)
                    {
                        continue;
                    }
                    if (values.Length != 4)
                    {
                        MessageBox.Show("PAS9 raw file is not in correct format. " + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    date = DateTime.ParseExact(values[0], "yyyy-MM-ddHH:mm:ss", null);
                    date = new DateTime(date.Year, date.Month, date.Day); //removing minutes

                    //Daily PPT value in mm
                    PPT_daily[0] = double.Parse(values[3]);

                    bool inserted_pptdata = false;
                    exists = checkIfDateExists(date, "p9_std_clim", "inserted_ppt_data", out inserted_pptdata, conn);

                    k++;
                    toolStripProgressBar1.PerformStep();

                    if (inserted_pptdata && radioButton1.Checked)
                        continue;

                    //exists = checkExistData(date, "p9_std_clim", conn);
                    sql = create_sql_of_StdP9(date, PPT_daily, exists, "P9_P", inserted_pptdata);

                    //-----------------------------end------------------------------------------------
                    try
                    {
                        cmd = new MySqlCommand(sql, conn);
                        int insertOrUpdate = cmd.ExecuteNonQuery();
                        db_rows_count = insertOrUpdate > 1 ? db_rows_count + 1 : db_rows_count + insertOrUpdate;

                    }
                    catch (MySqlException e)
                    {
                        MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    
                }
                MessageBox.Show(k + " records have been read from the raw file. " + db_rows_count + " rows have been effected in the database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            } //end of radiobutton3
            else if (radioButton5.Checked == true) ////in case of SoilMoist file
            {
                bool termin = false;
                DateTime date1, date2;
                double[] moistData_Avg = new double[15]; 
                double[,] moistdata = new double[24, 15];
                i = 0; string line="";
                StreamReader file = new StreamReader(@fileAddress);

                while (!termin)
                {
                    i++;

                    if (data == "")
                        line = file.ReadLine();

                    if (i == 1) continue;
                    
                    data = line;
                    data = data.Replace(" ", "");
                    data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                    data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                    string[] values = data.Split(',');

                    if (values.Length == 1)
                    {
                        continue;
                    }
                    if (values.Length != 17)
                    {
                        MessageBox.Show("PAS9 raw file is not in correct format. " + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    //date1 = DateTime.ParseExact(values[1], "MM/dd/yyyy", null);
                    date1 = Convert.ToDateTime(values[(int)columnIndex.CDATE]);

                    for (int j = 0; j < moistdata.GetLength(0); j++)
                        for (int m = 0; m < moistdata.GetLength(1); m++)
                            moistdata[j, m] = -99.99;
                    int n = 0;
                    do
                    {
                        //SoilMoisture_5_daily GRASS 
                        if (values[9].Equals(""))
                            moistdata[n, 0] = -99.99;
                        else
                            moistdata[n, 0] = double.Parse(values[9]);
                        //SoilMoisture_10_daily GRASS 
                        if (values[3].Equals(""))
                            moistdata[n, 1] = -99.99;
                        else
                            moistdata[n, 1] = double.Parse(values[3]);
                        //SoilMoisture_20_daily GRASS
                        if (values[6].Equals(""))
                            moistdata[n, 2] = -99.99;
                        else
                            moistdata[n, 2] = double.Parse(values[6]);
                        //SoilMoisture_50_daily GRASS
                        if (values[12].Equals(""))
                            moistdata[n, 3] = -99.99;
                        else
                            moistdata[n, 3] = double.Parse(values[12]);
                        //SoilMoisture_70_daily GRASS
                        if (values[15].Equals(""))
                            moistdata[n, 4] = -99.99;
                        else
                            moistdata[n, 4] = double.Parse(values[15]);

                        //SoilMoisture_5_daily BARE 
                        if (values[8].Equals(""))
                            moistdata[n, 5] = -99.99;
                        else
                            moistdata[n, 5] = double.Parse(values[8]);
                        //SoilMoisture_10_daily BARE 
                        if (values[2].Equals(""))
                            moistdata[n, 6] = -99.99;
                        else
                            moistdata[n, 6] = double.Parse(values[2]);
                        //SoilMoisture_20_daily BARE
                        if (values[5].Equals(""))
                            moistdata[n, 7] = -99.99;
                        else
                            moistdata[n, 7] = double.Parse(values[5]);
                        //SoilMoisture_50_daily BARE
                        if (values[11].Equals(""))
                            moistdata[n, 8] = -99.99;
                        else    
                            moistdata[n, 8] = double.Parse(values[11]);
                        //SoilMoisture_60_daily BARE
                        if (values[14].Equals(""))
                            moistdata[n, 9] = -99.99;
                        else
                            moistdata[n, 9] = double.Parse(values[14]);

                        //SoilMoisture_5_daily SHRUB 
                        if (values[10].Equals(""))
                            moistdata[n, 10] = -99.99;
                        else
                            moistdata[n, 10] = double.Parse(values[10]);
                        //SoilMoisture_10_daily SHRUB 
                        if (values[4].Equals(""))
                            moistdata[n, 11] = -99.99;
                        else
                            moistdata[n, 11] = double.Parse(values[4]);
                        //SoilMoisture_20_daily SHRUB
                        if (values[7].Equals(""))
                            moistdata[n, 12] = -99.99;
                        else
                            moistdata[n, 12] = double.Parse(values[7]);
                        //SoilMoisture_50_daily SHRUB
                        if (values[13].Equals(""))
                            moistdata[n, 13] = -99.99;
                        else
                            moistdata[n, 13] = double.Parse(values[13]);
                        //SoilMoisture_70_daily SHRUB
                        if (values[16].Equals(""))
                            moistdata[n, 14] = -99.99;
                        else
                            moistdata[n, 14] = double.Parse(values[16]);

                        if ((line = file.ReadLine()) != null)
                        {
                            data = line;
                            data = data.Replace(" ", "");
                            data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                            data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                            values = data.Split(',');

                            if (values.Length == 1)
                            {
                                termin = true;
                                break;
                            }
                            if (values.Length != 17)
                            {
                                MessageBox.Show("PAS9 raw file is not in correct format. " + values.Length + "values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                        else
                        {
                            termin = true;
                            break;
                        }

                        //date2 = DateTime.ParseExact(values[1], "MM/dd/yyyy", null);
                        date2 = Convert.ToDateTime(values[(int)columnIndex.CDATE]);
                        n++;
                        toolStripProgressBar1.PerformStep();

                    } while (date1 == date2);
                    //end of fetching data for one day

                    for (int j = 0; j < moistData_Avg.Length; j++)
                    {
                        moistData_Avg[j] = cal_avg_T(moistdata, j);
                    }
                    
                    bool inserted_moistdata = false;
                    exists = checkIfDateExists(date1, "p9_std_clim", "inserted_moist_data", out inserted_moistdata, conn);
                    
                    k++;

                    if (inserted_moistdata && radioButton1.Checked)
                        continue;

                    //exists = checkExistData(date1, "p9_std_clim", conn);
                    sql = create_sql_of_StdP9(date1, moistData_Avg, exists, "P9_M", inserted_moistdata);
                    
                    //-----------------------------end------------------------------------------------
                    try
                    {
                        cmd = new MySqlCommand(sql, conn);
                        int insertOrUpdate = cmd.ExecuteNonQuery();
                        db_rows_count = insertOrUpdate > 1 ? db_rows_count + 1 : db_rows_count + insertOrUpdate;
                    }
                    catch (MySqlException e)
                    {
                        MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    
                }
                MessageBox.Show(k + " records have been read from the raw file. " + db_rows_count + " rows have been effected in the database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }// end of radiobutton5

            toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        /*My Implementation-----Checked*/
        private void ImportGIBPE(string fileAddress)
        {
            int i = 0, j, k = 0, rows_affected = 0;
            DateTime date1, date2 = DateTime.MinValue;
            double tempDailyMin, tempDailyMax, tempDailyMean, tempDailyAvg, rhDailyMin, rhDailyMax, rhDailyMean, pptDailymm;
            string hrmin1 = "";

            Dictionary<string, double> dcTmp = new Dictionary<string, double>();
            Dictionary<string, double> dcRhm = new Dictionary<string, double>();
            Dictionary<string, double> dcPpt = new Dictionary<string, double>();

            MySqlConnection conn = null;
            MySqlCommand cmd = null;

            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //-----------------PROGRESS BAR setup---------------------------------
            var lines = File.ReadLines(fileAddress);
            int TotalLines = lines.Count();
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = TotalLines;
            //---------------------------------------------------------
            
            StreamReader file = new StreamReader(@fileAddress);
            string line = "", sql="";
            bool termin = false, skip = false, exists = false;
            string data = "";
            
            if(radioButton6.Checked == true) //in case of dailyTemp file
            {
                double[,] temprhppt = new double[289,3];
                k = 0; rows_affected = 0;
                
                while (!termin)
                {
                    i++;
                    if (data == "")
                        line = file.ReadLine();

                    if (i == 1 || i == 2 || i == 3 || i == 4) continue;
                    //remove minutes in date
                    data = line;
                    data = data.Replace(" ", "");
                    data = data.Replace("\"", "");
                    data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                    data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                    string[] values = data.Split(',');

                    if (values.Length == 1)
                    {
                        continue;
                    }
                    if (values.Length > 8)
                    {
                        MessageBox.Show("GIBPE raw file is not in correct format. " + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                
                    date1 = DateTime.ParseExact(values[0], "yyyy-MM-ddHH:mm:ss", null);
                    hrmin1 = date1.Hour.ToString() + ":" + date1.Minute.ToString();
                    date1 = new DateTime(date1.Year, date1.Month, date1.Day); //removing minutes
                    
                    int n = 0;

                    for (j = 0; j < temprhppt.GetLength(0); j++)
                        for (int m = 0; m < temprhppt.GetLength(1); m++)
                            temprhppt[j, m] = -99.99;

                    dcTmp.Clear();
                    dcRhm.Clear();
                    dcPpt.Clear();

                    do
                    {
                        if (!skip)
                        {   //temperature
                            //temprhppt[n, 0] = double.Parse(values[4]);
                            if (!dcTmp.ContainsKey(hrmin1))
                                dcTmp.Add(hrmin1, double.Parse(values[4]));
                            //humadity
                            //temprhppt[n, 1] = double.Parse(values[5]);
                            if (!dcRhm.ContainsKey(hrmin1))
                                dcRhm.Add(hrmin1, double.Parse(values[5]));
                            //ppt
                            if (values.Length == 7 && !(values[6].Contains("\0")))
                                //temprhppt[n, 2] = double.Parse(values[6]);
                                if (!dcPpt.ContainsKey(hrmin1))
                                    dcPpt.Add(hrmin1, double.Parse(values[6]));
                        }
                        if ((line = file.ReadLine()) != null)
                        {
                            data = line;
                            data = data.Replace(" ", "");
                            data = data.Replace("\"", "");
                            data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                            data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                            values = data.Split(',');
                            skip = false;

                            if (values.Length == 1)
                            {
                                skip = true;
                                continue;
                            }
                            if (values.Length > 8)
                            {
                                MessageBox.Show("GIBPE raw file is not in correct format. " + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                        else
                        {
                            termin = true;
                            break;
                        }

                        date2 = DateTime.ParseExact(values[0], "yyyy-MM-ddHH:mm:ss", null);
                        hrmin1 = date2.Hour.ToString() + ":" + date2.Minute.ToString();
                        date2 = new DateTime(date2.Year, date2.Month, date2.Day); //removing minutes 
                        
                        n++;
                        toolStripProgressBar1.PerformStep();

                    } while (date1 == date2);

                    j = 0;
                    foreach (var pair in dcTmp)
                    {
                        temprhppt[j, 0] = pair.Value;
                        j++;
                    }
                    
                    int value_count = 0;
                    double tempSum = 0.0;
                    double Min = temprhppt[0, 0];
                    double Max = temprhppt[0, 0];

                    for (j = 0; j < temprhppt.GetLength(0); j++)
                    {
                        if (temprhppt[j, 0] == -99.99)
                            continue;
                        tempSum += temprhppt[j, 0];
                        value_count++;
                        if (temprhppt[j, 0] > Max)
                            Max = temprhppt[j, 0];
                        if (temprhppt[j, 0] < Min)
                            Min = temprhppt[j, 0];
                    }

                    tempDailyMin = Min;
                    tempDailyMax = Max;

                    tempDailyMean = (tempDailyMax + tempDailyMin)/2;

                    if (tempSum != 0.0)
                        tempDailyAvg = tempSum / value_count;
                    else
                        tempDailyAvg = -99.99;


                    j = 0;
                    foreach (var pair in dcRhm)
                    {
                        temprhppt[j, 1] = pair.Value;
                        j++;
                    }

                    Min = temprhppt[0, 1];
                    Max = temprhppt[0, 1];

                    for (j = 0; j < temprhppt.GetLength(0); j++)
                    {
                        if (temprhppt[j, 0] == -99.99)
                            continue;
                        if (temprhppt[j, 1] > Max)
                            Max = temprhppt[j, 1];
                        if (temprhppt[j, 1] < Min)
                            Min = temprhppt[j, 1];
                    }

                    rhDailyMax = Max;
                    rhDailyMin = Min;

                    rhDailyMean = (rhDailyMax + rhDailyMin) / 2;
                    pptDailymm = 0.0;

                    j = 0;
                    foreach (var pair in dcPpt)
                    {
                        temprhppt[j, 2] = pair.Value;
                        j++;
                    }

                    for (j = 0, value_count = 0; j < temprhppt.GetLength(0); j++)
                    {
                        if (temprhppt[j, 2] == -99.99)
                            continue;
                        pptDailymm += temprhppt[j, 2];
                        value_count++;
                    }
                    if (value_count != 0)
                        pptDailymm = pptDailymm / 0.039370;
                    else
                        pptDailymm = -99.99;

                    double Vsat, Vair, VPD;
                    //calculating VPD
                    if (tempDailyAvg != -99.99 && rhDailyMean != -99.99)
                    {
                        Vsat = 610.7 * (Math.Pow(10,((7.5 * tempDailyAvg) / (tempDailyAvg + 237.3))));
                        Vair = (1 - (rhDailyMean / 100)) * Vsat;
                        VPD = Vair / 1000;
                    }
                    else
                        VPD = -99.99;

                    double AGDD = -99.99;

                    double[] datavalues = new double[11];
                    datavalues[0] = tempDailyMin;   datavalues[1] = tempDailyMax;   datavalues[2] = tempDailyMean;
                    datavalues[3] = tempDailyAvg;   datavalues[4] = pptDailymm;
                    datavalues[5] = rhDailyMin;     datavalues[6] = rhDailyMax;     datavalues[7] = rhDailyMean;
                    datavalues[8] = VPD; datavalues[9] = AGDD; datavalues[10] = this.dayLengthCalculator(date1, "gibpe");

                    bool inserted_tempdata = false;
                    exists = checkIfDateExists(date1, "gi_std_clim", "inserted_temp_data", out inserted_tempdata, conn);
                    //exists = checkExistData(date1, "gi_std_clim", conn);
                    
                    k++;

                    if (inserted_tempdata && radioButton1.Checked)
                        continue;

                    sql = create_sql_of_StdGIBPE(date1, datavalues, exists, "GI_T", inserted_tempdata);

                    try
                    {
                        cmd = new MySqlCommand(sql, conn);
                        int insertOrUpdate = cmd.ExecuteNonQuery();
                        rows_affected = insertOrUpdate > 1 ? rows_affected + 1 : rows_affected + insertOrUpdate;

                    }
                    catch (MySqlException e)
                    {
                        MessageBox.Show("MySQL Error: " + sql + " " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    
                } //end of while
                MessageBox.Show(k + " records have been read from the raw file. " + rows_affected + " rows have been effected in the database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //MessageBox.Show(rows_affected + " rows updated to database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            } //end of radiobuttion6 
            else if (radioButton5.Checked == true) //in case of soilMoist file
            {
                double[] moistData_Avg = new double[15];
                double[,] moistdata = new double[30, 15];
                i = 0;
                k = 0; rows_affected = 0;

                while (!termin)
                {
                    i++;

                    if (data == "")
                        line = file.ReadLine();

                    if (i == 1) continue;

                    data = line;
                    data = data.Replace(" ", "");
                    data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                    data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                    string[] values = data.Split(',');

                    if (values.Length == 1)
                    {
                        continue;
                    }
                    if (values.Length != 30)
                    {
                        MessageBox.Show("GIBPE raw file is not in correct format. " + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    //date1 = DateTime.ParseExact(values[1], "MM/dd/yyyy", null);
                    date1 = Convert.ToDateTime(values[(int)columnIndex.CDATE]);

                    for (j = 0; j < moistdata.GetLength(0); j++)
                        for (int m = 0; m < moistdata.GetLength(1); m++)
                            moistdata[j, m] = -99.99;
                    int n = 0;
                    do
                    {
                        //SoilMoisture_5_daily GRASS 
                        if (values[23]=="-99.99" && !(values[24]=="-99.99"))
                            values[23] = "";
                        if (values[24]=="-99.99" && !(values[23]=="-99.99"))
                            values[24] = "";
                        if (values[23].Equals("") && values[24].Equals(""))
                            moistdata[n, 0] = -99.99;
                        else if (!values[23].Equals("") && !values[24].Equals(""))
                            moistdata[n, 0] = (double.Parse(values[23]) + double.Parse(values[24]))/2;
                        else if(!values[23].Equals(""))
                            moistdata[n, 0] = double.Parse(values[23]);
                        else if (!values[24].Equals(""))
                            moistdata[n, 0] = double.Parse(values[24]);

                        //SoilMoisture_10_daily GRASS 
                        if (values[4]=="-99.99" && !(values[5]=="-99.99"))
                            values[4] = "";
                        if (values[5]=="-99.99" && !(values[4]=="-99.99"))
                            values[5] = "";
                        if (values[4].Equals("") && values[5].Equals(""))
                            moistdata[n, 1] = -99.99;
                        else if (!values[4].Equals("") && !values[5].Equals(""))
                            moistdata[n, 1] = (double.Parse(values[4]) + double.Parse(values[5])) / 2;
                        else if (!values[4].Equals(""))
                            moistdata[n, 1] = double.Parse(values[4]);
                        else if (!values[5].Equals(""))
                            moistdata[n, 1] = double.Parse(values[5]);

                        //SoilMoisture_20_daily GRASS
                        if (values[10]=="-99.99" && !(values[11]=="-99.99"))
                            values[10] = "";
                        if (values[11]=="-99.99" && !(values[10]=="-99.99"))
                            values[11] = "";

                        if (values[10].Equals("") && values[11].Equals(""))
                            moistdata[n, 2] = -99.99;
                        else if (!values[10].Equals("") && !values[11].Equals(""))
                            moistdata[n, 2] = (double.Parse(values[10]) + double.Parse(values[11])) / 2;
                        else if (!values[10].Equals(""))
                            moistdata[n, 2] = double.Parse(values[10]);
                        else if (!values[11].Equals(""))
                            moistdata[n, 2] = double.Parse(values[11]);

                        //SoilMoisture_30_daily GRASS
                        if (values[16].Equals(""))
                            moistdata[n, 3] = -99.99;
                        else
                            moistdata[n, 3] = double.Parse(values[16]);
                        
                        //SoilMoisture_50_daily GRASS
                        if (values[27].Equals(""))
                            moistdata[n, 4] = -99.99;
                        else
                            moistdata[n, 4] = double.Parse(values[27]);

                        //SoilMoisture_5_daily BARE 
                        if (values[21]=="-99.99" && !(values[22]=="-99.99"))
                            values[21] = "";
                        if (values[22]=="-99.99" && !(values[21]=="-99.99"))
                            values[22] = "";
                        if (values[21].Equals("") && values[22].Equals(""))
                            moistdata[n, 5] = -99.99;
                        else if (!values[21].Equals("") && !values[22].Equals(""))
                            moistdata[n, 5] = (double.Parse(values[21]) + double.Parse(values[22])) / 2;
                        else if (!values[21].Equals(""))
                            moistdata[n, 5] = double.Parse(values[21]);
                        else if (!values[22].Equals(""))
                            moistdata[n, 5] = double.Parse(values[22]);

                        //SoilMoisture_10_daily BARE 
                        if (values[2]=="-99.99" && !(values[3]=="-99.99"))
                            values[2] = "";
                        if (values[3]=="-99.99" && !(values[2]=="-99.99"))
                            values[3] = "";
                        if (values[2].Equals("") && values[3].Equals(""))
                            moistdata[n, 6] = -99.99;
                        else if (!values[2].Equals("") && !values[3].Equals(""))
                            moistdata[n, 6] = (double.Parse(values[2]) + double.Parse(values[3])) / 2;
                        else if (!values[2].Equals(""))
                            moistdata[n, 6] = double.Parse(values[2]);
                        else if (!values[3].Equals(""))
                            moistdata[n, 6] = double.Parse(values[3]);

                        //SoilMoisture_20_daily BARE
                        if (values[8]=="-99.99" && !(values[9]=="-99.99"))
                            values[8] = "";
                        if (values[9]=="-99.99" && !(values[8]=="-99.99"))
                            values[9] = "";
                        if (values[8].Equals("") && values[9].Equals(""))
                            moistdata[n, 7] = -99.99;
                        else if (!values[8].Equals("") && !values[9].Equals(""))
                            moistdata[n, 7] = (double.Parse(values[8]) + double.Parse(values[9])) / 2;
                        else if (!values[8].Equals(""))
                            moistdata[n, 7] = double.Parse(values[8]);
                        else if (!values[9].Equals(""))
                            moistdata[n, 7] = double.Parse(values[9]);

                        //SoilMoisture_30_daily BARE
                        if (values[14]=="-99.99" && !(values[15]=="-99.99"))
                            values[14] = "";
                        if (values[15]=="-99.99" && !(values[14]=="-99.99"))
                            values[15] = "";
                        if (values[14].Equals("") && values[15].Equals(""))
                            moistdata[n, 8] = -99.99;
                        else if (!values[14].Equals("") && !values[15].Equals(""))
                            moistdata[n, 8] = (double.Parse(values[14]) + double.Parse(values[15])) / 2;
                        else if (!values[14].Equals(""))
                            moistdata[n, 8] = double.Parse(values[14]);
                        else if (!values[15].Equals(""))
                            moistdata[n, 8] = double.Parse(values[15]);

                        //SoilMoisture_35_daily BARE
                        if (values[19]=="-99.99" && !(values[20]=="-99.99"))
                            values[19] = "";
                        if (values[20]=="-99.99" && !(values[19]=="-99.99"))
                            values[20] = "";
                        if (values[19].Equals("") && values[20].Equals(""))
                            moistdata[n, 9] = -99.99;
                        else if (!values[19].Equals("") && !values[20].Equals(""))
                            moistdata[n, 9] = (double.Parse(values[19]) + double.Parse(values[20])) / 2;
                        else if (!values[19].Equals(""))
                            moistdata[n, 9] = double.Parse(values[19]);
                        else if (!values[20].Equals(""))
                            moistdata[n, 9] = double.Parse(values[20]);

                        //SoilMoisture_5_daily SHRUB 
                        if (values[25]=="-99.99" && !(values[26]=="-99.99"))
                            values[25] = "";
                        if (values[26]=="-99.99" && !(values[25]=="-99.99"))
                            values[26] = "";
                        if (values[25].Equals("") && values[26].Equals(""))
                            moistdata[n, 10] = -99.99;
                        else if (!values[25].Equals("") && !values[26].Equals(""))
                            moistdata[n, 10] = (double.Parse(values[25]) + double.Parse(values[26])) / 2;
                        else if (!values[25].Equals(""))
                            moistdata[n, 10] = double.Parse(values[25]);
                        else if (!values[26].Equals(""))
                            moistdata[n, 10] = double.Parse(values[26]);

                        //SoilMoisture_10_daily SHRUB 
                        if (values[6]=="-99.99" && !(values[7]=="-99.99"))
                            values[6] = "";
                        if (values[7]=="-99.99" && !(values[6]=="-99.99"))
                            values[7] = "";
                        if (values[6].Equals("") && values[7].Equals(""))
                            moistdata[n, 11] = -99.99;
                        else if (!values[6].Equals("") && !values[7].Equals(""))
                            moistdata[n, 11] = (double.Parse(values[6]) + double.Parse(values[7])) / 2;
                        else if (!values[6].Equals(""))
                            moistdata[n, 11] = double.Parse(values[6]);
                        else if (!values[7].Equals(""))
                            moistdata[n, 11] = double.Parse(values[7]);

                        //SoilMoisture_20_daily SHRUB
                        if (values[12]=="-99.99" && !(values[13]=="-99.99"))
                            values[12] = "";
                        if (values[13]=="-99.99" && !(values[12]=="-99.99"))
                            values[13] = "";
                        if (values[12].Equals("") && values[13].Equals(""))
                            moistdata[n, 12] = -99.99;
                        else if (!values[12].Equals("") && !values[13].Equals(""))
                            moistdata[n, 12] = (double.Parse(values[12]) + double.Parse(values[13])) / 2;
                        else if (!values[12].Equals(""))
                            moistdata[n, 12] = double.Parse(values[12]);
                        else if (!values[13].Equals(""))
                            moistdata[n, 12] = double.Parse(values[13]);

                        //SoilMoisture_30_daily SHRUB
                        if (values[17]=="-99.99" && !(values[18]=="-99.99"))
                            values[17] = "";
                        if (values[18]=="-99.99" && !(values[17]=="-99.99"))
                            values[18] = "";
                        if (values[17].Equals("") && values[18].Equals(""))
                            moistdata[n, 13] = -99.99;
                        else if (!values[17].Equals("") && !values[18].Equals(""))
                            moistdata[n, 13] = (double.Parse(values[17]) + double.Parse(values[18])) / 2;
                        else if (!values[17].Equals(""))
                            moistdata[n, 13] = double.Parse(values[17]);
                        else if (!values[18].Equals(""))
                            moistdata[n, 13] = double.Parse(values[18]);

                        //SoilMoisture_50_daily SHRUB
                        if (values[28]=="-99.99" && !(values[29]=="-99.99"))
                            values[28] = "";
                        if (values[29]=="-99.99" && !(values[28]=="-99.99"))
                            values[29] = "";
                        if (values[28].Equals("") && values[29].Equals(""))
                            moistdata[n, 14] = -99.99;
                        else if (!values[28].Equals("") && !values[29].Equals(""))
                            moistdata[n, 14] = (double.Parse(values[28]) + double.Parse(values[29])) / 2;
                        else if (!values[28].Equals(""))
                            moistdata[n, 14] = double.Parse(values[28]);
                        else if (!values[29].Equals(""))
                            moistdata[n, 14] = double.Parse(values[29]);

                        if ((line = file.ReadLine()) != null)
                        {
                            data = line;
                            data = data.Replace(" ", "");
                            data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                            data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                            values = data.Split(',');

                            if (values.Length == 1)
                            {
                                termin = true;
                                break;
                            }
                            if (values.Length != 30)
                            {
                                MessageBox.Show("GIBPE raw file is not in correct format. " + values.Length + "values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                        else
                        {
                            termin = true;
                            break;
                        }

                        //date2 = DateTime.ParseExact(values[1], "MM/dd/yyyy", null);
                        date2 = Convert.ToDateTime(values[(int)columnIndex.CDATE]);
                        n++;
                        toolStripProgressBar1.PerformStep();

                    } while (date1 == date2);
                    //end of fetching data for one day

                    for (j = 0; j < moistData_Avg.Length; j++)
                    {
                        moistData_Avg[j] = cal_avg_T(moistdata, j);
                    }

                    bool inserted_moistdata = false;
                    exists = checkIfDateExists(date1, "gi_std_clim", "inserted_moist_data", out inserted_moistdata, conn);

                    k++;

                    if (inserted_moistdata && radioButton1.Checked)
                        continue;

                    //exists = checkExistData(date1, "gi_std_clim", conn);
                    sql = create_sql_of_StdGIBPE(date1, moistData_Avg, exists, "GI_M", inserted_moistdata);

                    //-----------------------------end------------------------------------------------
                    try
                    {
                        cmd = new MySqlCommand(sql, conn);
                        int insertOrUpdate = cmd.ExecuteNonQuery();
                        rows_affected = insertOrUpdate > 1 ? rows_affected + 1 : rows_affected + insertOrUpdate;
                    }
                    catch (MySqlException e)
                    {
                        MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    
                } //end of outer while
                MessageBox.Show(k + " records have been read from the raw file. " + rows_affected + " rows have been effected in the database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }//end of radiobuttion5
 
            toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }
        /*My Implementation----Checked*/
        private void ImportCRN(string fileAddress)
        {
            int k = 0, db_row_count = 0;
            double[] MoistTemp = new double[10];
            string sql = null;

            double TdailyMax, TdailyMin, TdailyMean, TdailyAvg, RhDailyMax, RhDailyMin, RhDailyMean;
            double SurTempDailyMax, SurTempDailyMin, SurTempDailyAvg, PdailyCalc, SolaradDaily, Svp, Vpd, VpdAvg;
 
            MySqlConnection conn = null;
            MySqlTransaction trans = null;
            
            //-----------------PROGRESS BAR setup---------------------------------
            var lines = File.ReadLines(fileAddress);
            int TotalLines = lines.Count();
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = TotalLines;
            //---------------------------------------------------------

            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (string line in File.ReadLines(@fileAddress))
            {
                string data = line;
                string[] values = Regex.Split(data, @"\s+");
                
                if (values.Length == 1)
                {
                    continue;
                }
                if (values.Length != 28)
                {
                    MessageBox.Show("CRN raw file is not in correct format." + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                
                string WbanNo = values[0];
                DateTime date = DateTime.ParseExact(values[1], "yyyyMMdd", null);
                float CrxVn = float.Parse(values[2]);
                
                double Longitude = double.Parse(values[3]);
                double Latitude = double.Parse(values[4]);
                if (double.Parse(values[5]) != -9999.0)
                    TdailyMax = double.Parse(values[5]);
                else
                    TdailyMax = -99.99;
                if(double.Parse(values[6]) != -9999.0) 
                    TdailyMin = double.Parse(values[6]);
                else
                    TdailyMin = -99.99;
                if(double.Parse(values[7]) != -9999.0)
                    TdailyMean = double.Parse(values[7]);
                else
                    TdailyMean = -99.99;
                if(double.Parse(values[8]) != -9999.0) 
                    TdailyAvg = double.Parse(values[8]);
                else
                    TdailyAvg = -99.99;
                //--------------------------------------------
                if(double.Parse(values[9]) != -9999.0)
                    PdailyCalc = double.Parse(values[9]);
                else
                    PdailyCalc = -99.99;
                if(double.Parse(values[10]) != -9999.0)
                    SolaradDaily = double.Parse(values[10]);
                else
                    SolaradDaily = -99.99;
                
                string SurTempDailyType = values[11];
                //---------------------------------------------
                if (double.Parse(values[12]) != -9999.0)
                    SurTempDailyMax = double.Parse(values[12]);
                else
                    SurTempDailyMax = -99.99;
                if(double.Parse(values[13]) != -9999.0)
                    SurTempDailyMin = double.Parse(values[13]);
                else
                    SurTempDailyMin = -99.99;
                if (double.Parse(values[14]) != -9999.0)
                    SurTempDailyAvg = double.Parse(values[14]);
                else
                    SurTempDailyAvg = -99.99;
                //----------------------------------------------
                if(double.Parse(values[15]) != -9999.0)
                    RhDailyMax = double.Parse(values[15]);
                else
                    RhDailyMax = -99.99;
                if (double.Parse(values[16]) != -9999.0)
                    RhDailyMin = double.Parse(values[16]);
                else
                    RhDailyMin = -99.99;

                if (RhDailyMax != -99.99 && RhDailyMin != -99.99)
                    RhDailyMean = (RhDailyMax + RhDailyMin) / 2;
                else
                    RhDailyMean = -99.99;

                /**********************************
                if (double.Parse(values[17]) != -9999.0)
                    RhDailyMean = double.Parse(values[17]);
                else
                    RhDailyMean = -99.99;
                 *************************************/
                //----------------------------------------------
                for (int j = 0; j < MoistTemp.Length; j++)
                {
                    if (j < 5 && double.Parse(values[18 + j]) != -99.000)
                        MoistTemp[j] = double.Parse(values[18 + j]);
                    else if (j >= 5 && double.Parse(values[18 + j]) != -9999.0)
                        MoistTemp[j] = double.Parse(values[18 + j]);
                    else
                        MoistTemp[j] = -99.99;
                }
                //----------------------------------------------
                //VPD Formula, SVP (Pascals) = 610.7*10^(7.5T/(237.3+T))
                
                //Calculate VPD
                if (TdailyAvg != -99.99 && RhDailyMean != -99.99)
                {
                    Svp = 610.7 * (Math.Pow(10, ((7.5 * TdailyAvg) / (TdailyAvg + 237.3))));
                    Vpd = (1 - (RhDailyMean / 100)) * Svp;
                    VpdAvg = Vpd / 1000;
                }
                else
                    VpdAvg = -99.99;

                double AGDD = -99.99;

                //Calculate DAY LENGTH
                double dayLength = this.dayLengthCalculator(date, "crn");
                
                string siteCode = get_site_code("CRN");

                ArrayList sqlCommands = new ArrayList();
                
                /*
                //create sql for raw data table
                sql = replaceOrignore + " cr_raw_clim ";
                sql = sql + "VALUES('" +
                    date.ToString("yyyy-MM-dd") + "'," + WbanNo + "," + CrxVn + "," + Longitude + "," + Latitude + "," +
                    TdailyMax + "," + TdailyMin + "," + TdailyMean + "," + TdailyAvg + "," + PdailyCalc + "," +
                    SolaradDaily + ",'" + SurTempDailyType + "'," + SurTempDailyMax + "," + SurTempDailyMin + "," + SurTempDailyAvg + "," +
                    RhDailyMax + "," + RhDailyMin + "," + RhDailyMean + ","; 
                    
                for (int j = 0; j < MoistTemp.Length; j++)
                    sql = sql + MoistTemp[j] + ",";
                sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql = sql + ")";
                //--------------------------------------
                
                sqlCommands.Add(sql);
                */
                
                //create sql for std data table
                ArrayList columns = get_column_names("CRN");

                sql = replaceOrignore + " cr_std_clim(";
                foreach (string col in columns)
                {
                    sql = sql + col + ",";
                }
                sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql += ")";

                sql = sql + " VALUES('" + date.ToString("yyyy-MM-dd") + "','" + siteCode + "',"+TdailyMin + "," + TdailyMax + "," + TdailyMean + "," + TdailyAvg + ","+
                     PdailyCalc + "," + RhDailyMin + "," + RhDailyMax + "," + RhDailyMean + "," + VpdAvg + "," + AGDD + "," + dayLength + ",";

                for (int j = 0; j < MoistTemp.Length; j++)
                {
                    sql = sql + MoistTemp[j] + ",";
                }
                sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql += ")";
                //-------------------------------------
                sqlCommands.Add(sql);
                //-----------------------------end------------------------------------------------
                k++;
                if (checkExistData(date, "cr_std_clim", conn) && radioButton1.Checked)
                    continue; 
                
                try
                {
                    trans = conn.BeginTransaction();

                    foreach (string commandString in sqlCommands)
                    {
                        MySqlCommand cmd = new MySqlCommand(commandString, conn, trans);
                        int insertOrUpdate = cmd.ExecuteNonQuery();
                        db_row_count = insertOrUpdate > 1 ? db_row_count+1 : db_row_count + insertOrUpdate;
                    }

                    trans.Commit(); 
                }
                catch (MySqlException e)
                {
                    MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                
                toolStripProgressBar1.PerformStep();
            } //end of for each

            MessageBox.Show(k + " records have been read from the raw file. " + db_row_count + " rows have been effected in the database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (conn.State == ConnectionState.Open)
                conn.Close();
            toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
        }

        /*My Implementation*/
        private bool checkExistTwe(DateTime date, MySqlConnection con)
        {
            string sql = "SELECT date FROM tw_std_clim WHERE date = '" + date.ToString("yyyy-MM-dd") + "'"; 
            MySqlCommand check = new MySqlCommand(sql, con);
            MySqlDataReader rdr = check.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Close();
                return true;
            }
            else
            {
                rdr.Close();
                return false;
            }
        }
        private bool checkExistData(DateTime date, string table, MySqlConnection con)
        {
            string sql = "SELECT date FROM " + table + " WHERE date = '" + date.ToString("yyyy-MM-dd") + "'";
            if (con.State == ConnectionState.Closed)
                con.Open();
            MySqlCommand check = new MySqlCommand(sql, con);
            MySqlDataReader rdr = check.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Close();
                return true;
            }
            else
            {
                rdr.Close();
                return false;
            }
        }

        private bool checkIfDateExists(DateTime date, string table, string bool_field, out bool bool_result, MySqlConnection con)
        {
            string sql = "SELECT date,"+ bool_field + " FROM " + table + " WHERE date = '" + date.ToString("yyyy-MM-dd") + "'";
                            
            if (con.State == ConnectionState.Closed)
                con.Open();
            MySqlCommand check = new MySqlCommand(sql, con);
            MySqlDataReader rdr = check.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                bool_result = (rdr.GetInt16(1) == 1) ? true : false;
                rdr.Close();
                return true;
            }
            else
            {
                rdr.Close();
                bool_result = false;
                return false;
            }
        }

        private string create_sql_of_StdGIBPE(DateTime date, double[] ptvalues, bool exists, string type, bool inserted)
        {
            string sql = "", extra = "";

            ArrayList columns = get_column_names(type);
            for (int j = 0; j < columns.Count; j++)
            {
                if (columns[j].Equals("SOIL_MOISTURE_50_DAILY_GRASS"))
                {
                    columns[j] = "SOIL_MOISTURE_30_DAILY_GRASS";
                }
                if (columns[j].Equals("SOIL_MOISTURE_100_DAILY_GRASS"))
                {
                    columns[j] = "SOIL_MOISTURE_50_DAILY_GRASS";
                }
                if (columns[j].Equals("SOIL_MOISTURE_50_DAILY_BARE"))
                {
                    columns[j] = "SOIL_MOISTURE_30_DAILY_BARE";
                }
                if (columns[j].Equals("SOIL_MOISTURE_100_DAILY_BARE"))
                {
                    columns[j] = "SOIL_MOISTURE_35_DAILY_BARE";
                }
                if (columns[j].Equals("SOIL_MOISTURE_50_DAILY_SHRUB"))
                {
                    columns[j] = "SOIL_MOISTURE_30_DAILY_SHRUB";
                }
                if (columns[j].Equals("SOIL_MOISTURE_100_DAILY_SHRUB"))
                {
                    columns[j] = "SOIL_MOISTURE_50_DAILY_SHRUB";
                }
                
            }

            if (type.Equals("GI_T"))
                extra = "inserted_temp_data";
            else
                extra = "inserted_moist_data";

            if (exists) //have to update the rows
            {
                sql = "UPDATE " + "gi_std_clim" + " SET ";
                for (int j = 0; j < ptvalues.Length; j++)
                    sql = sql + columns[j + 2] + " = " + ptvalues[j] + ",";
                
                if (!inserted)
                    sql = sql + extra + " = 1";
                else
                    sql = sql.Remove(sql.LastIndexOf(','), 1);
                
                sql = sql + " WHERE date = '" + date.ToString("yyyy-MM-dd") + "'";
            }
            else    //have to insert rows
            {
                sql = replaceOrignore + " gi_std_clim" + "(";
                foreach (string col in columns)
                {
                    sql = sql + col + ",";
                }
                //sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql += extra; 

                sql += ")";
                
                string siteCode = get_site_code("GIBPE");

                sql = sql + " VALUES('" + date.ToString("yyyy-MM-dd") + "','" + siteCode +"'," ;
                for (int i = 0; i < ptvalues.Length; i++)
                {
                    sql = sql + ptvalues[i] + ",";
                }
                //sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql += "1";
                
                sql += ")";
            }
            return sql;
        }

        private string create_sql_of_StdP9(DateTime date, double[] ptvalues, bool exists, string type, bool inserted)
        {
            string sql = "", extra = "";

            ArrayList columns = get_column_names(type);

            if (type.Equals("P9_T"))
                extra = "inserted_temp_data";
            else if (type.Equals("P9_M"))
                extra = "inserted_moist_data";
            else if (type.Equals("P9_P"))
                extra = "inserted_ppt_data";
            else
                extra = "has_gi_rh_data";

            for (int j = 0; j < columns.Count; j++)
            {
                if (columns[j].Equals("SOIL_MOISTURE_100_DAILY_GRASS"))
                {
                    columns[j] = "SOIL_MOISTURE_70_DAILY_GRASS";
                }
                if (columns[j].Equals("SOIL_MOISTURE_100_DAILY_BARE"))
                {
                    columns[j] = "SOIL_MOISTURE_60_DAILY_BARE";
                }
                if (columns[j].Equals("SOIL_MOISTURE_100_DAILY_SHRUB"))
                {
                    columns[j] = "SOIL_MOISTURE_70_DAILY_SHRUB";
                }
                if (columns[j].Equals("RH_DAILY_MIN"))
                {
                    columns[j] = "GI_RH_DAILY_MIN";
                }
                if (columns[j].Equals("RH_DAILY_MAX"))
                {
                    columns[j] = "GI_RH_DAILY_MAX";
                }
                if (columns[j].Equals("RH_DAILY_MEAN"))
                {
                    columns[j] = "GI_RH_DAILY_MEAN";
                }
            }
            if (exists) //have to update the rows
            {
                sql = "UPDATE " + "p9_std_clim" + " SET ";
                for (int j = 0; j < ptvalues.Length; j++)
                    sql = sql + columns[j + 2] + " = " + ptvalues[j] + ",";
                
                if (!inserted)
                    sql = sql + extra + " = 1";
                else
                    sql = sql.Remove(sql.LastIndexOf(','), 1);
                
                sql = sql + " WHERE date = '" + date.ToString("yyyy-MM-dd") + "'";
            }
            else    //have to insert rows
            {
                sql = replaceOrignore + " p9_std_clim" + "(";
                foreach (string col in columns)
                {
                    sql = sql + col + ",";
                }

                sql += extra;
                //sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql += ")";
                
                string siteCode = get_site_code("PAS9");

                sql = sql + " VALUES('" + date.ToString("yyyy-MM-dd") + "','" + siteCode + "',";

                for (int i = 0; i < ptvalues.Length; i++)
                {
                    sql = sql + ptvalues[i] + ",";
                }
                
                sql += "1";
                //sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql += ")";
            }
            return sql;
        }
        /*My Implementation*/
        private string create_sql_of_StdTWE(DateTime date, double[] ptvalues, bool exists, string type, bool inserted)
        {
            string sql = "", extra = "";

            ArrayList columns = get_column_names(type);

            if (type.Equals("TWE_T"))
                extra = "inserted_temp_data";
            else
                extra = "inserted_moist_data";

            if (exists) //have to update the rows
            {
                sql = "UPDATE " + "tw_std_clim" + " SET ";
                for (int j = 0; j < ptvalues.Length; j++)
                    sql = sql + columns[j + 2] + " = " + ptvalues[j] + ",";
                
                if(!inserted)
                    sql = sql + extra + " = 1";
                else
                    sql = sql.Remove(sql.LastIndexOf(','), 1);
                
                sql = sql + " WHERE date = '" + date.ToString("yyyy-MM-dd") + "'";
            }
            else    //have to insert rows
            {
                sql = replaceOrignore + " tw_std_clim" + "(";
                foreach (string col in columns)
                {
                    sql = sql + col + ",";
                }
                //sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql += extra;
                
                sql += ")";
                
                string siteCode = get_site_code("TWEE");
                
                sql = sql + " VALUES('" + date.ToString("yyyy-MM-dd") + "','" + siteCode + "',";

                for (int i = 0; i < ptvalues.Length; i++)
                {
                    sql = sql + ptvalues[i] + ",";
                }
                sql += "1";

                //sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql += ")";
            }
            return sql;
        }

        /*My Implementation-----Checked*/
        private void ImportTWE(string fileAddress)
        {
            int i = 0, k = 0, db_rows_count = 0;
            bool exists = false;
            double Svp, Vpd;

            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            
            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //-----------------PROGRESS BAR setup---------------------------------
            var lines = File.ReadLines(fileAddress);
            int TotalLines = lines.Count();
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = TotalLines;
            //---------------------------------------------------------

            StreamReader file = new StreamReader(@fileAddress);
            
            string line = "";
            string data = "", sql = "";
            
            if(radioButton6.Checked == true) //in case of dailyTemp file
            {
                double[] ppt_temp_values = new double[11];
                while ((line = file.ReadLine()) != null)
                {
                    i++;
                    
                    if (i == 1) continue;

                    data = line;
                    data = data.Replace(" ", "");
                    data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                    data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                    string[] values = data.Split(',');
                
                    if (values.Length == 1)
                    {
                        continue;
                    }
                    if (values.Length != 11)
                    {
                        MessageBox.Show("TWEE raw file is not in correct format. " + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    //date
                    DateTime date = Convert.ToDateTime(values[(int)columnIndex.CDATE]);

                    //tdailymin
                    ppt_temp_values[0] = double.Parse(values[2]);
                    //tdailymax
                    ppt_temp_values[1] = double.Parse(values[3]);
                    //tDailyMean
                    ppt_temp_values[2] = double.Parse(values[4]); 
                    //tdailyavg
                    ppt_temp_values[3] = double.Parse(values[5]);
                    //pptdailymm
                    ppt_temp_values[4] = double.Parse(values[6]);
                    //rhdailymin  
                    ppt_temp_values[5] = double.Parse(values[7]);
                    //rhdailymax
                    ppt_temp_values[6] = double.Parse(values[8]);
                    //rhDailyMean
                    ppt_temp_values[7] = double.Parse(values[9]);

                    //Calculate VPD
                    if (ppt_temp_values[3] != -99.99 && ppt_temp_values[7] != -99.99)
                    {
                        Svp = 610.7 * (Math.Pow(10, (7.5 * ppt_temp_values[3]) / (ppt_temp_values[3] + 237.3)));
                        Vpd = (1 - (ppt_temp_values[7] / 100)) * Svp;
                        ppt_temp_values[8] = Vpd / 1000;
                    }
                    else
                        ppt_temp_values[8] = -99.99;
                    
                    //AGDD
                    ppt_temp_values[9] = -99.99;
                    //DAY LENGTH
                    ppt_temp_values[10] = this.dayLengthCalculator(date, "tweedie");


                    bool inserted_tempdata = false;
                    exists = checkIfDateExists(date, "tw_std_clim", "inserted_temp_data", out inserted_tempdata, conn);

                    k++;  
                    //exists = checkExistTwe(date, conn);
                    toolStripProgressBar1.PerformStep();

                    if (inserted_tempdata && radioButton1.Checked)
                        continue;

                    sql = create_sql_of_StdTWE(date, ppt_temp_values, exists, "TWE_T", inserted_tempdata);
    
                    //-----------------------------end------------------------------------------------
                    try
                    {
                        cmd = new MySqlCommand(sql, conn);
                        int insertOrUpdate = cmd.ExecuteNonQuery();
                        db_rows_count = insertOrUpdate > 1 ? db_rows_count + 1 : db_rows_count + insertOrUpdate;
                    }
                    catch (MySqlException e)
                    {
                        MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                     
                }//end of while

                MessageBox.Show(k + " records have been read from the raw file. " + db_rows_count + " rows have been effected in the database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }//end of radiobutton6
            else if (radioButton5.Checked == true) //in case of dailyMoisture file
            {
                bool termin = false;
                db_rows_count = 0;
                DateTime date1, date2;
                double[] moist_temp = new double[6]; 
                double[,] moistandtemp = new double[24, 6];
                
                while (!termin)
                {
                    i++;
                    
                    if (data == "")
                        line = file.ReadLine();
                    
                    if (i == 1) continue;
                    data = line;

                    data = data.Replace(" ", "");
                    data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                    data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);
                    
                    string[] values = data.Split(',');

                    if (values.Length == 1)
                    {
                        continue;
                    }
                    if (values.Length != 10)
                    {
                        MessageBox.Show("TWEE raw file is not in correct format. " + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    date1 = DateTime.ParseExact(values[(int)CRNcolumIndex.CDATE], "yyyy-MM-ddHH:mm:ss", null);
                    //date1 = DateTime.ParseExact(values[(int)CRNcolumIndex.CDATE], "M/d/yyyyH:mm", null);
                    date1 = new DateTime(date1.Year, date1.Month, date1.Day); //removing minutes

                    for (int j = 0; j < moistandtemp.GetLength(0); j++)
                        for (int m = 0; m < moistandtemp.GetLength(1); m++)
                            moistandtemp[j, m] = -99.99;
                    int n = 0;
                    do
                    {
                        // 5 cm moisture
                        moistandtemp[n, 0] = double.Parse(values[3]);
                        // 10 cm moisture
                        moistandtemp[n, 1] = double.Parse(values[4]);
                        // 20 cm moisture
                        moistandtemp[n, 2] = double.Parse(values[5]);
                        // 5 cm temperature
                        moistandtemp[n, 3] = double.Parse(values[7]);
                        // 10 cm temperature
                        moistandtemp[n, 4] = double.Parse(values[8]);
                        // 20 cm temperature
                        moistandtemp[n, 5] = double.Parse(values[9]);

                        if ((line = file.ReadLine()) != null)
                        {
                            data = line;
                            data = data.Replace(" ", "");
                            data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                            data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                            values = data.Split(',');
                            if (values.Length == 1)
                            {
                                termin = true;
                                break;
                            }
                            if (values.Length != 10)
                            {
                                MessageBox.Show("TWEE raw file is not in correct format. " + values.Length + "values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                        else
                        {
                            termin = true;
                            break;
                        }
                        //date2 = DateTime.ParseExact(values[(int)CRNcolumIndex.CDATE], "M/d/yyyyH:mm", null);
                        date2 = DateTime.ParseExact(values[(int)CRNcolumIndex.CDATE], "yyyy-MM-ddHH:mm:ss", null);
                        date2 = new DateTime(date2.Year, date2.Month, date2.Day);
                        
                        n++;
                        toolStripProgressBar1.PerformStep();
                    
                    } while (date1 == date2);
                    //end of fetching data for one day

                    for (int j = 0; j < moist_temp.Length; j++)
                    {
                        moist_temp[j] = cal_avg_T(moistandtemp, j);
                    }

                    //exists = checkExistTwe(date1, conn);
                    bool inserted_moistdata = false;
                    exists = checkIfDateExists(date1, "tw_std_clim", "inserted_moist_data", out inserted_moistdata, conn);
                    
                    k++;   
                    
                    if (inserted_moistdata && radioButton1.Checked)
                        continue;

                    sql = create_sql_of_StdTWE(date1, moist_temp, exists, "TWE_M", inserted_moistdata);
                
                    //-----------------------------end------------------------------------------------
                    try
                    {
                        cmd = new MySqlCommand(sql, conn);
                        int insertOrUpdate = cmd.ExecuteNonQuery();
                        db_rows_count = insertOrUpdate > 1 ? db_rows_count + 1 : db_rows_count + insertOrUpdate;

                    }
                    catch (MySqlException e)
                    {
                        MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                   
                }//end of while inside radiobutton5

                MessageBox.Show(k + " records have been read from the raw file. " + db_rows_count + " rows have been effected in the database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }//end of radiobutton5

            toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
            if(conn.State == ConnectionState.Open)
                conn.Close();
        }

        /*My Implementation---Checked*/
        private void ImportTROM(string fileAddress)
        {
            int i = 0, j, k = 0, n, value_count, db_row_count = 0;
            string line = "", data ="";
            bool termin = false;
            DateTime date1, date2;
            double tempSum, tdailymin, tdailymax, tDailyMean, tdailyavg, rhdailymin, rhdailymax, rhDailyMean, pptDaily;
            double Svp, Vpd, VpdAvg;
            double[,] moistandtemp = new double[48,8];
            double[] pptdailymm = new double[48];
            double[] moist_temp = new double[8]; 

            ArrayList airTempHourly = new ArrayList();
            ArrayList rhumHourly = new ArrayList();

            MySqlConnection conn = null;

            //-----------------PROGRESS BAR setup----------------------
            var lines = File.ReadLines(fileAddress);
            int TotalLines = lines.Count();
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = TotalLines;
            //---------------------------------------------------------
            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("MySQL Error: "+ e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //----------------------------------------------------------

            StreamReader file = new StreamReader(@fileAddress);
            
            while (!termin)
            {
                i++;
                if (data == "")
                    line = file.ReadLine();
                if (i == 1 || i == 2 || i == 3) continue;

                data = line;
                data = data.Replace(" ", "");
                data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                string[] values = data.Split(',');
                string str_date = values[1]+"/"+values[2]+"/"+values[0];
                date1 = DateTime.ParseExact(str_date, "M/d/yyyy", null);

                if (values.Length == 1)
                {
                    continue;
                }
                if (values.Length != 17)
                {
                    MessageBox.Show("TROM raw file is not in correct format. " + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                                
                tdailyavg = 0.0;
               
                for (j = 0; j < moistandtemp.GetLength(0); j++)
                    for(int m = 0; m < moistandtemp.GetLength(1); m++) 
                        moistandtemp[j,m] = -99.99;

                for (j = 0; j < pptdailymm.Length; j++)
                    pptdailymm[j] = -99.99;
    
                n = 0;
                do 
                {
                    //get hourly air temp
                    airTempHourly.Add(double.Parse(values[10]));
                    //get hourly rel. humidity
                    rhumHourly.Add(double.Parse(values[11]));
                    //get pptdaily
                    pptdailymm[n] = double.Parse(values[12]);                    
                    // 5 cm moisture
                    moistandtemp[n,0] = double.Parse(values[6]);
                    // 15 cm moisture
                    moistandtemp[n,1] = double.Parse(values[7]);               
                    // 30 cm moisture
                    moistandtemp[n,2] = double.Parse(values[8]);                   
                    // 50 cm moisture
                    moistandtemp[n,3] = double.Parse(values[9]);                
                    // 5 cm temperature
                    moistandtemp[n,4] = double.Parse(values[13]);                   
                    // 15 cm temperature
                    moistandtemp[n,5] = double.Parse(values[14]);                  
                    // 30 cm temperature
                    moistandtemp[n,6] = double.Parse(values[15]);                  
                    // 50 cm temperature
                    moistandtemp[n,7] = double.Parse(values[16]);
                    
                    if ((line = file.ReadLine()) != null)
                    {
                        data = line;
                        data = data.Replace(" ", "");
                        data = Regex.Replace(data, "NAN", "-99.99", RegexOptions.IgnoreCase);
                        data = Regex.Replace(data, "NA", "-99.99", RegexOptions.IgnoreCase);

                        values = data.Split(',');
                        if (values.Length == 1)
                        {
                            termin = true;
                            break;
                        }
                        if (values.Length != 17)
                        {
                            MessageBox.Show("TROM raw file is not in correct format. " + values.Length + "values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        string str_date2 = values[1] + "/" + values[2] + "/" + values[0];
                        date2 = DateTime.ParseExact(str_date2, "M/d/yyyy", null);
                    }
                    else
                    {
                        termin = true;
                        break;
                    }
                    toolStripProgressBar1.PerformStep();
                    n++;
                }while (date1 == date2);
                //end of fetching data for one day

                // Convert ArrayList to array.
                double[] airTempArr = airTempHourly.ToArray(typeof(double)) as double[];
                double[] rhumArr = rhumHourly.ToArray(typeof(double)) as double[];
                //-------------------TEMP-------------------------
                if (airTempHourly.Count == 0)
                {
                    tdailymin = -99.99;
                    tdailymax = -99.99;
                    tdailyavg = -99.99;
                }
                else
                {
                    double tempMin = -99.99,tempMax = -99.99; 
                    value_count = 0;
                    tempSum = 0.0;
                    for(j = 0; j < airTempArr.Length; j++)
                    {
                        if (airTempArr[j] != -99.99)
                        {
                            tempMin = airTempArr[j];
                            tempMax = airTempArr[j];
                            break;
                        }
                    }
                    for (j = 0; j < airTempArr.Length; j++)
                    {
                        if (airTempArr[j] == -99.99)
                             continue;
                        tempSum += airTempArr[j];
                        value_count++;
                        if (airTempArr[j] > tempMax)
                            tempMax = airTempArr[j];
                        if (airTempArr[j] < tempMin)
                            tempMin = airTempArr[j];
                    }

                    tdailymin = tempMin;
                    tdailymax = tempMax;

                    if (tempSum != 0.0)
                        tdailyavg = tempSum / value_count;
                    else
                        tdailyavg = -99.99;
                }

                airTempHourly.Clear();

                tDailyMean = (tdailymin + tdailymax) / 2;
                //-------------------RH-----------------------
                if (rhumHourly.Count == 0)
                {
                    rhdailymin = -99.99;
                    rhdailymax = -99.99;
                }
                else
                {
                    double rhMin = -99.99, rhMax = -99.99;
                    value_count = 0;
                    tempSum = 0.0;
                    for (j = 0; j < rhumArr.Length; j++)
                    {
                        if (rhumArr[j] != -99.99)
                        {
                            rhMin = rhumArr[j];
                            rhMax = rhumArr[j];
                            break;
                        }
                    }
                    for (j = 0; j < rhumArr.Length; j++)
                    {
                        if (rhumArr[j] == -99.99)
                            continue;
                        tempSum += rhumArr[j];
                        value_count++;
                        if (rhumArr[j] > rhMax)
                            rhMax = rhumArr[j];
                        if (rhumArr[j] < rhMin)
                            rhMin = rhumArr[j];
                    }

                    rhdailymin = rhMin;
                    rhdailymax = rhMax;
                }
                rhumHourly.Clear();

                rhDailyMean = (rhdailymin + rhdailymax) / 2;
                
                //-------------------PPT--------------------
                value_count = 0;
                tempSum = 0.0;
                for (j = 0; j < pptdailymm.Length; j++)
                {
                    if (pptdailymm[j] == -99.99)
                        continue;
                    tempSum += pptdailymm[j];
                    value_count++;
                }
                if (value_count == 0)
                    pptDaily = -99.99;
                else
                    pptDaily = tempSum; 
                //-------------------------------------------
                for (j = 0; j < moist_temp.Length; j++)
                {
                    moist_temp[j] = cal_avg_T(moistandtemp, j);
                }

                //Calculate VPD
                if (tdailyavg != -99.99 && rhDailyMean != -99.99)
                {
                    Svp = 610.7 * (Math.Pow(10, ((7.5 * tdailyavg) / (tdailyavg + 237.3))));
                    Vpd = (1 - (rhDailyMean / 100)) * Svp;
                    VpdAvg = Vpd / 1000;
                }
                else
                    VpdAvg = -99.99;

                double AGDD = -99.99;
                string siteCode = get_site_code("TROM");

                //CALCULATE DAY LENGTH
                double dayLength = this.dayLengthCalculator(date1, "tromble");

                ArrayList columns = get_column_names("TRM");
                string sql = replaceOrignore + " tr_std_clim("; 
                foreach (string col in columns)
                {
                    sql = sql + col + ",";
                }
                sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql += ")";

                sql = sql + " VALUES('" + date1.ToString("yyyy-MM-dd") + "','" + siteCode + "'," + tdailymin + "," + tdailymax + "," + tDailyMean + "," + tdailyavg + "," +
                    pptDaily + "," + rhdailymin + "," + rhdailymax + "," + rhDailyMean + "," + VpdAvg + "," + AGDD + "," + dayLength + ",";

                for (j = 0; j < moist_temp.Length; j++)
                {
                    sql = sql + moist_temp[j] + ",";
                }
                sql = sql.Remove(sql.LastIndexOf(','), 1);
                sql += ")";

                k++;
                if (checkExistData(date1, "tr_std_clim", conn) && radioButton1.Checked)
                    continue; 
                //-----------------------------end------------------------------------------------
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    int insertOrUpdate = cmd.ExecuteNonQuery();
                    db_row_count = insertOrUpdate > 1 ? db_row_count + 1 : db_row_count + insertOrUpdate;


                }
                catch (MySqlException e)
                {
                    MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                

            } //end of while

            MessageBox.Show(k + " records have been read from the raw file. " + db_row_count + " rows have been effected in the database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (conn.State == ConnectionState.Open)
                conn.Close();
            toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
        }
        /*My Implementation*/
        private double cal_avg_T(double[,] moisttemp, int fieldindx)
        {
            double sum = 0.0, avg = 0.0;
            int val_count = 0;
            
            for (int j = 0; j < moisttemp.GetLength(0); j++)
            {
                if (moisttemp[j, fieldindx] == -99.99)
                    continue;
                sum += moisttemp[j, fieldindx];
                val_count++;
            }
            if (val_count != 0)
                avg = sum / val_count;
            else
                avg = -99.99;
            return avg;
        }
        private string get_site_code(string sitename)
        {
            string sitecode = "";
            switch (sitename)
            {
                case "CRN":
                    sitecode = "CR";
                    break;
                case "SCAN":
                    sitecode = "SC";
                    break;
                case "TROM":
                    sitecode = "TR";
                    break;
                case "TWEE":
                    sitecode = "TW";
                    break;
                case "PAS9":
                    sitecode = "P9";
                    break;
                case "IBPE":
                case "GIBPE":
                    sitecode = "GI";
                    break;      
            }
            return sitecode;
        }
        
        /*My Implementation*/
        private ArrayList get_column_names(string site)
        {
            ArrayList columns = new ArrayList();
            string[] columnNames = {    "DATE", //0
                                        "SITE_CODE", //1
                                        "T_DAILY_MIN", //2
                                        "T_DAILY_MAX", //3
                                        "T_DAILY_MEAN", //4
                                        "T_DAILY_AVG", //5
                                        "PPT_DAILY_MM", //6
                                        "RH_DAILY_MIN", //7
                                        "RH_DAILY_MAX", //8 
                                        "RH_DAILY_MEAN", //9
                                        "VPD_DAILY_AVG", //10
                                        "AGDD", //11
                                        "DAY_LENGTH",  //12
                                        "SOIL_MOISTURE_5_DAILY_DUNE",  //13
                                        "SOIL_MOISTURE_10_DAILY_DUNE", //14
                                        "SOIL_MOISTURE_20_DAILY_DUNE", //15
                                        "SOIL_MOISTURE_50_DAILY_DUNE", //16
                                        "SOIL_MOISTURE_100_DAILY_DUNE", //17
                                        "SOIL_MOISTURE_5_DAILY_GRASS",  //18
                                        "SOIL_MOISTURE_10_DAILY_GRASS", //19
                                        "SOIL_MOISTURE_20_DAILY_GRASS", //20
                                        "SOIL_MOISTURE_50_DAILY_GRASS", //21
                                        "SOIL_MOISTURE_100_DAILY_GRASS", //22
                                        "SOIL_MOISTURE_5_DAILY_BARE",  //23
                                        "SOIL_MOISTURE_10_DAILY_BARE", //24
                                        "SOIL_MOISTURE_20_DAILY_BARE", //25
                                        "SOIL_MOISTURE_50_DAILY_BARE", //26
                                        "SOIL_MOISTURE_100_DAILY_BARE", //27
                                        "SOIL_MOISTURE_5_DAILY_SHRUB", //28
                                        "SOIL_MOISTURE_10_DAILY_SHRUB", //29
                                        "SOIL_MOISTURE_20_DAILY_SHRUB", //30
                                        "SOIL_MOISTURE_50_DAILY_SHRUB", //31
                                        "SOIL_MOISTURE_100_DAILY_SHRUB", //32
                                        "SOIL_TEMP_5_DAILY_DUNE",   //33
                                        "SOIL_TEMP_10_DAILY_DUNE",  //34
                                        "SOIL_TEMP_20_DAILY_DUNE",  //35
                                        "SOIL_TEMP_50_DAILY_DUNE",  //36
                                        "SOIL_TEMP_100_DAILY_DUNE", //37
                                        "SOIL_TEMP_5_DAILY_GRASS",  //38
                                        "SOIL_TEMP_10_DAILY_GRASS", //39
                                        "SOIL_TEMP_20_DAILY_GRASS", //40
                                        "SOIL_TEMP_50_DAILY_GRASS", //41
                                        "SOIL_TEMP_100_DAILY_GRASS", //42
                                        "SOIL_TEMP_5_DAILY_BARE",  //43
                                        "SOIL_TEMP_10_DAILY_BARE", //44
                                        "SOIL_TEMP_20_DAILY_BARE", //45
                                        "SOIL_TEMP_50_DAILY_BARE", //46
                                        "SOIL_TEMP_100_DAILY_BARE", //47
                                        "SOIL_MOISTURE_5_DAILY_BARE", //48
                                        "SOIL_MOISTURE_15_DAILY_BARE", //49
                                        "SOIL_MOISTURE_30_DAILY_BARE", //50
                                        "SOIL_MOISTURE_50_DAILY_BARE", //51
                                        "SOIL_TEMP_5_DAILY_BARE",   //52
                                        "SOIL_TEMP_15_DAILY_BARE",  //53
                                        "SOIL_TEMP_30_DAILY_BARE",  //54
                                        "SOIL_TEMP_50_DAILY_BARE",  //55
                                  };
            bool[] toinsert = new bool[columnNames.Length];
            for (int i = 0; i < toinsert.Length; i++)
            {
                toinsert[i] = true;
            }
            switch (site)
            {
                case "CRN":
                    for (int j = 0; j < 10; j++)
                    {
                        toinsert[13 + j] = false;
                    }
                    for (int j = 0; j < 15; j++)
                    {
                        toinsert[28 + j] = false;
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        toinsert[48 + j] = false;
                    }
                    break;
                case "TRM":
                    for (int j = 0; j < 35; j++)
                    {
                        toinsert[13 + j] = false;
                    }
                    break;
                case "TWE_T": //for temperature file
                    for (int j = 0; j < 43; j++)
                    {
                        toinsert[13 + j] = false;
                    }
                    break;
                case "TWE_M": //for moisture file
                    for (int j = 0; j < 21; j++)
                    {
                        toinsert[2 + j] = false;
                    }
                    for (int j = 0; j < 17; j++)
                    {
                        toinsert[26 + j] = false;
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        toinsert[46 + j] = false;
                    }
                    break;
                case "GI_T": //for temperature file
                    for (int j = 0; j < 43; j++)
                    {
                        toinsert[13 + j] = false;
                    }
                    break;
                case "GI_M": //for moisture file
                    for (int j = 0; j < 16; j++)
                    {
                        toinsert[2 + j] = false;
                    }
                    for (int j = 0; j < 23; j++)
                    {
                        toinsert[33 + j] = false;
                    }
                    break;
                case "P9_M": //for moisture file
                    for (int j = 0; j < 16; j++)
                    {
                        toinsert[2 + j] = false;
                    }
                    for (int j = 0; j < 23; j++)
                    {
                        toinsert[33 + j] = false;
                    }
                    break;
                case "P9_T": //for temperature file
                    for (int j = 0; j < 5; j++)
                    {
                        toinsert[6 + j] = false;
                    }
                    for (int j = 0; j < 43; j++)
                    {
                        toinsert[13 + j] = false;
                    }
                    break;
                case "P9_P": //for ppt file
                    for (int j = 0; j < 4; j++)
                    {
                        toinsert[2 + j] = false;
                    }
                    for (int j = 0; j < 49; j++)
                    {
                        toinsert[7 + j] = false;
                    }
                    break;
                case "P9_R": //for RH update
                    for (int j = 0; j < 5; j++)
                    {
                        toinsert[2 + j] = false;
                    }
                    for (int j = 0; j < 45; j++)
                    {
                        toinsert[11 + j] = false;
                    }
                    break;
            }

            for (int i = 0; i < columnNames.Length; i++)
            {
                if (toinsert[i])
                     columns.Add(columnNames[i]);
            }

            return columns;

        }
        
        /*My Implementation--- Checked*/
        private void ImportSCAN(string fileAddress)
        {
            //PPT_DAILY is percipitation that needs to be converted from Inch to mm
            //-1 in index array means that data does not exist in the file and we need to calculate it.
            int i=0, k = 0, db_rows_count = 0;
            double tDailyMean, rhDailyMean, Svp, Vpd, VpdAvg;
            DateTime date1, date2;
 
            MySqlConnection conn = null;
            MySqlCommand cmd = null;

            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //-----------------PROGRESS BAR setup----------------------
            var lines = File.ReadLines(fileAddress);
            int TotalLines = lines.Count();
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = TotalLines;
            //---------------------------------------------------------
            
            StreamReader file = new StreamReader(@fileAddress);
            string line="";
            bool termin = false;
            string data = "";
            while (!termin)
            {
                i++;
                if(data=="")
                    line = file.ReadLine();
                if (i == 1 || i == 2|| i==3) continue;

                data = line;
                data = data.Replace(" ", "");
                data = Regex.Replace(data, "NAN", "-99.9", RegexOptions.IgnoreCase);

                string[] values = data.Split(',');
                
                if (values.Length == 1)
                {
                    continue;
                }
                if (values.Length != 98)
                {
                    MessageBox.Show("SCAN raw file is not in correct format. "+ values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                //date
                date1 = Convert.ToDateTime(values[(int)columnIndex.CDATE]);
                double tdailymin = Double.MaxValue;
                double tdailymax = Double.MinValue;
                double tdailyavg = 0.0;
                double pptdailymm = -99.9;
                double rhdailymin = Double.MaxValue;
                double rhdailymax = Double.MinValue;
               // double [] soilandtemp = new double[30];
                double[,] soilandtemp = new double[30,2]; //column is used for storing count of values

                int tdailyavg_count = 0;

                //Initialize soilandtemp count
                for(int soilTempCount1 = 0; soilTempCount1<soilandtemp.GetLength(0);soilTempCount1++)
                    soilandtemp[soilTempCount1, 1] = 0;


                do
                {
                    //get absolute tdailymin
                    if (double.Parse(values[(int)columnIndex.T_DAILY_MIN]) != -99.9 && tdailymin > double.Parse(values[(int)columnIndex.T_DAILY_MIN]))
                        tdailymin = double.Parse(values[(int)columnIndex.T_DAILY_MIN]);

                    //get absolute tdailymax
                    if (double.Parse(values[(int)columnIndex.T_DAILY_MAX]) != -99.9 && tdailymax < double.Parse(values[(int)columnIndex.T_DAILY_MAX]))
                        tdailymax = double.Parse(values[(int)columnIndex.T_DAILY_MAX]);

                    //get tdaliyavg
                    if (double.Parse(values[(int)columnIndex.T_DAILY_AVG]) != -99.9)
                    {
                        tdailyavg += double.Parse(values[(int)columnIndex.T_DAILY_AVG]);
                        tdailyavg_count++;
                    }
                    //get pptdaily
                    if (double.Parse(values[(int)columnIndex.PPT_DAILY]) != -99.9)
                    {
                        if (pptdailymm == -99.9)
                        {
                            pptdailymm = double.Parse(values[(int)columnIndex.PPT_DAILY]);
                        }
                        else
                        {
                            pptdailymm += double.Parse(values[(int)columnIndex.PPT_DAILY]);
                        }
                    }
                    //get absolute rhdailymin
                    if (double.Parse(values[(int)columnIndex.RH_DAILY_MIN]) != -99.9 && rhdailymin > double.Parse(values[(int)columnIndex.RH_DAILY_MIN]))
                        rhdailymin = double.Parse(values[(int)columnIndex.RH_DAILY_MIN]);

                    //get absolute rhdailymax
                    if (double.Parse(values[(int)columnIndex.RH_DAILY_MAX]) != -99.9 && rhdailymax < double.Parse(values[(int)columnIndex.RH_DAILY_MAX]))
                        rhdailymax = double.Parse(values[(int)columnIndex.RH_DAILY_MAX]);

                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_5_DAILY_DUNE]) != -99.9)
                        { soilandtemp[0,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_5_DAILY_DUNE]);
                          soilandtemp[0,1]+=1.0;}
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_10_DAILY_DUNE]) != -99.9)
                        {  soilandtemp[1,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_10_DAILY_DUNE]);
                            soilandtemp[1, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_20_DAILY_DUNE]) != -99.9)
                        { soilandtemp[2,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_20_DAILY_DUNE]);
                            soilandtemp[2, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_50_DAILY_DUNE]) != -99.9)
                        {  soilandtemp[3,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_50_DAILY_DUNE]);
                            soilandtemp[3, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_100_DAILY_DUNE]) != -99.9)
                        { soilandtemp[4,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_100_DAILY_DUNE]);
                            soilandtemp[4, 1] += 1.0;
                        }

                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_5_DAILY_GRASS]) != -99.9)
                        { soilandtemp[5,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_5_DAILY_GRASS]);
                            soilandtemp[5, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_10_DAILY_GRASS]) != -99.9)
                        { soilandtemp[6,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_10_DAILY_GRASS]);
                            soilandtemp[6, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_20_DAILY_GRASS]) != -99.9)
                        { soilandtemp[7,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_20_DAILY_GRASS]);
                            soilandtemp[7, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_50_DAILY_GRASS]) != -99.9)
                        { soilandtemp[8,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_50_DAILY_GRASS]);
                            soilandtemp[8, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_100_DAILY_GRASS]) != -99.9)
                        { soilandtemp[9,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_100_DAILY_GRASS]);
                            soilandtemp[9, 1] += 1.0;
                        }

                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_5_DAILY_BARE]) != -99.9)
                        { soilandtemp[10,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_5_DAILY_BARE]);
                            soilandtemp[10, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_10_DAILY_BARE]) != -99.9)
                        { soilandtemp[11,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_10_DAILY_BARE]);
                            soilandtemp[11, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_20_DAILY_BARE]) != -99.9)
                        { soilandtemp[12,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_20_DAILY_BARE]);
                            soilandtemp[12, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_50_DAILY_BARE]) != -99.9)
                        { soilandtemp[13,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_50_DAILY_BARE]);
                            soilandtemp[13, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_MOISTURE_100_DAILY_BARE]) != -99.9)
                        { soilandtemp[14,0] += double.Parse(values[(int)columnIndex.SOIL_MOISTURE_100_DAILY_BARE]);
                            soilandtemp[14, 1] += 1.0;
                        }

                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_5_DAILY_DUNE]) != -99.9)
                        { soilandtemp[15,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_5_DAILY_DUNE]);
                            soilandtemp[15, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_10_DAILY_DUNE]) != -99.9)
                        { soilandtemp[16,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_10_DAILY_DUNE]);
                            soilandtemp[16, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_20_DAILY_DUNE]) != -99.9)
                        { soilandtemp[17,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_20_DAILY_DUNE]);
                            soilandtemp[17, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_50_DAILY_DUNE]) != -99.9)
                        { soilandtemp[18,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_50_DAILY_DUNE]);
                            soilandtemp[18, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_100_DAILY_DUNE]) != -99.9)
                        { soilandtemp[19,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_100_DAILY_DUNE]);
                            soilandtemp[19, 1] += 1.0;
                        }

                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_5_DAILY_GRASS]) != -99.9)
                        { soilandtemp[20,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_5_DAILY_GRASS]);
                            soilandtemp[20, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_10_DAILY_GRASS]) != -99.9)
                        { soilandtemp[21,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_10_DAILY_GRASS]);
                            soilandtemp[21, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_20_DAILY_GRASS]) != -99.9)
                        { soilandtemp[22,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_20_DAILY_GRASS]);
                            soilandtemp[22, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_50_DAILY_GRASS]) != -99.9)
                        { soilandtemp[23,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_50_DAILY_GRASS]);
                            soilandtemp[23, 1] += 1.0;
                           }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_100_DAILY_GRASS]) != -99.9)
                        { soilandtemp[24,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_100_DAILY_GRASS]);
                            soilandtemp[24, 1] += 1.0;
                        }

                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_5_DAILY_BARE]) != -99.9)
                        { soilandtemp[25,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_5_DAILY_BARE]);
                            soilandtemp[25, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_10_DAILY_BARE]) != -99.9)
                        { soilandtemp[26,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_10_DAILY_BARE]);
                            soilandtemp[26, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_20_DAILY_BARE]) != -99.9)
                        { soilandtemp[27,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_20_DAILY_BARE]);
                            soilandtemp[27, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_50_DAILY_BARE]) != -99.9)
                        { soilandtemp[28,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_50_DAILY_BARE]);
                            soilandtemp[28, 1] += 1.0;
                        }
                    if (double.Parse(values[(int)columnIndex.SOIL_TEMP_100_DAILY_BARE]) != -99.9)
                        { soilandtemp[29,0] += double.Parse(values[(int)columnIndex.SOIL_TEMP_100_DAILY_BARE]);
                            soilandtemp[29, 1] += 1.0;
                        }

                   

                    if ((line = file.ReadLine()) != null)
                    {
                        data = line;
                        data = data.Replace(" ", "");
                        data = Regex.Replace(data, "NAN", "-99.9", RegexOptions.IgnoreCase);
                        values = data.Split(',');

                        if (values.Length == 1)
                        {
                            termin = true;
                            break;
                        }
                        if (values.Length != 98)
                        {
                            MessageBox.Show("SCAN raw file is not in correct format. " + values.Length + " values found", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }

                        date2 = Convert.ToDateTime(values[(int)columnIndex.CDATE]);
                    }
                    else
                    {
                        termin = true;
                        break;
                    }
                    toolStripProgressBar1.PerformStep();

                } while (date1 == date2); //end of while (fetching data for each day)

                if (tdailyavg_count != 0)
                    tdailyavg = tdailyavg / tdailyavg_count;
                else
                    tdailyavg = -99.99;

                if (tdailymin != Double.MaxValue && tdailymax != Double.MinValue)
                    tDailyMean = (tdailymin + tdailymax) / 2;
                else
                    tDailyMean = -99.99;

                if (rhdailymin != Double.MaxValue && rhdailymax != Double.MinValue)
                    rhDailyMean = (rhdailymax + rhdailymin) / 2;
                else
                    rhDailyMean = -99.99;

                //replace soilandtemp by mean
                for (int soilTempCount1 = 0; soilTempCount1 < soilandtemp.GetLength(0); soilTempCount1++)
                {
                    if(soilandtemp[soilTempCount1, 1]==0) // check divide by 0
                    {
                        soilandtemp[soilTempCount1, 0] = -99.99;
                        continue;
                    }
                    soilandtemp[soilTempCount1, 0] = soilandtemp[soilTempCount1, 0] / soilandtemp[soilTempCount1, 1];
                }

                //Calculate VPD
                if (tdailyavg != -99.99 && rhDailyMean != -99.99)
                {
                    Svp = 610.7 * (Math.Pow(10, ((7.5 * tdailyavg) / (tdailyavg + 237.3))));
                    Vpd = (1 - (rhDailyMean / 100)) * Svp;
                    VpdAvg = Vpd / 1000;
                }
                else
                    VpdAvg = -99.99;

                //percipitation conversion from Inch to MM
                double mmPPT_DAILY = -99.99;
                if (pptdailymm != -99.9)
                {
                    mmPPT_DAILY = pptdailymm / 0.039370;
                }

                double AGDD = -99.99;
                string siteCode = get_site_code("SCAN");
                //CALCULATE DAY LENGTH
                double dayLength = this.dayLengthCalculator(date1, "scan");

                string sql = replaceOrignore + " sc_std_clim VALUES('" +

                   date1.ToString("yyyy-MM-dd") + "','" +
                   siteCode + "'," +

                   tdailymin + "," + tdailymax + "," + tDailyMean + "," + tdailyavg + "," +
                   mmPPT_DAILY + "," +
                   rhdailymin + "," + rhdailymax + "," + rhDailyMean + "," +

                   VpdAvg + "," + AGDD + "," + dayLength + ",";
                   
                   for (int j = 0; j < soilandtemp.GetLength(0); j++)
                   {
                        sql = sql + soilandtemp[j,0] + ",";
                   }
                   sql = sql.Remove(sql.LastIndexOf(','), 1);
                   sql += ")";
                
                
                k++;
                if (checkExistData(date1, "sc_std_clim", conn) && radioButton1.Checked)
                    continue;
                //-----------------------------end------------------------------------------------
                try
                {
                    cmd = new MySqlCommand(sql, conn);
                    int insertOrUpdate = cmd.ExecuteNonQuery();
                    db_rows_count = insertOrUpdate > 1 ? db_rows_count+1 : db_rows_count + insertOrUpdate;
                }
                catch (MySqlException e)
                {
                    MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    break;
                }
               
                
            }// end of outer while

            MessageBox.Show(k + " records have been read from the raw file. " + db_rows_count + " rows have been effected in the database", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if(conn.State == ConnectionState.Open)
                conn.Close();
            toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
        }

        private void lockUnlockGUI(string l)
        {
            if (l == "Lock")
            {
                button1.Enabled = false;
                textBox1.Enabled = false;
                comboBox1.Enabled = false;
                button2.Enabled = false;
                button9.Enabled = false;
                panel1.Enabled = false;
                panel2.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
                textBox1.Enabled = true;
                comboBox1.Enabled = true;
                button2.Enabled = true;
                button9.Enabled = true;
                panel1.Enabled = true;
                panel2.Enabled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.dailySolarNoonDumper(textBox2.Text);
        }

        private void dailySolarNoonDumper(string fileAddress)
        {
            directoryForDialog = fileAddress;
            string solarNoonConnectionString = "";
            int i = 0;
            //string solarNoonConnectionString = "server=jornada-vdbmy.nmsu.edu; user id=phenology_admin; password=phenoCam1; database=phenologydb; pooling=false";
            //string solarNoonConnectionString = "server=localhost; user id=root; password= Jrn.phenomet;database=phenomet; pooling=false"; 

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["phenologyDBConnection"];

            // If found, return the connection string. 
            if (settings != null)
                solarNoonConnectionString = settings.ConnectionString;

            MySqlConnection c = null;
            
            //-----------------PROGRESS BAR setup----------------------
            var lines = File.ReadLines(fileAddress);
            int TotalLines = lines.Count();
            toolStripProgressBar2.Step = 1;
            toolStripProgressBar2.Minimum = 0;
            toolStripProgressBar2.Maximum = TotalLines;
            //---------------------------------------------------------
            try
            {
                c = new MySqlConnection(solarNoonConnectionString);
                //c = new MySqlConnection(connString);
                c.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (string line in File.ReadLines(@fileAddress))
            {
                i++;

                if (i == 1) continue;

                //remove minutes in date
                string data = line;
                data = data.Replace(" ", "");
                data = data.Replace("NaN", "-99.9");
                string[] values = data.Split(',');
                
                if (values.Length == 1)
                {
                    continue;
                }
                if (values.Length != 15)
                {
                    MessageBox.Show("Solar Noon file is not in correct format", "File Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
               //get the ROI from file name
                string ROI=fileAddress.Substring(fileAddress.LastIndexOf('\\')+1);
                ROI = ROI.Substring(ROI.IndexOf('_')+1, ROI.LastIndexOf('_') - ROI.IndexOf('_')-1);

                //GET date from first column
                string date = values[0];
                date = date.Substring(date.IndexOf('_') + 1, date.LastIndexOf('.') - date.IndexOf('_'));

                DateTime d;
                try
                {
                    d = new DateTime(int.Parse(date.Split('_')[0]), int.Parse(date.Split('_')[1].Substring(0, 2)), int.Parse(date.Split('_')[1].Substring(2, 2)));
                }
                catch (Exception e)
                {
                    MessageBox.Show("Date Parsing Warning:"+e.Message, "Date Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }
                
                //GET camera name from first column
                string cameraName = values[0];
                cameraName = cameraName.Substring(0, cameraName.IndexOf('_'));

                //GET siteCode from siteinfo table
                string siteinfosql = "SELECT Sitecode FROM siteinfo WHERE Sitename='" + cameraName.Substring(0, cameraName.Length - 2) + "'";
                MySqlCommand getSiteCode = new MySqlCommand(siteinfosql, c);
                MySqlDataReader rdr=getSiteCode.ExecuteReader();
                rdr.Read();
                string siteCode = rdr.GetString(0);
                rdr.Close();

                string sql = S_replaceOrignore + " dailysolarnoonvis values('" + cameraName + "','" + siteCode + "','" + ROI + "','" + double.Parse(values[1].ToString()) + "','" + double.Parse(values[2].ToString()) + "','" +
                    double.Parse(values[3].ToString()) + "','" + double.Parse(values[4].ToString()) + "','" + double.Parse(values[5].ToString()) + "','" + double.Parse(values[6].ToString()) + "','" +
                    double.Parse(values[7].ToString())+"','"+double.Parse(values[8].ToString())+"','"+double.Parse(values[10].ToString())+"','"+double.Parse(values[11].ToString())+"','"+double.Parse(values[12].ToString())+"','"+double.Parse(values[13].ToString())+"','"+
                    double.Parse(values[14].ToString()) + "','" + d.ToString("yyyy-MM-dd") + "','" + values[0].ToString() + "','" + double.Parse(values[9].ToString())+"')";
                
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, c);
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException e)
                {
                    MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                toolStripProgressBar2.PerformStep();
                
            } //end of for each loop

            MessageBox.Show((i - 1) + " Rows are imported to database successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (c.State == ConnectionState.Open)
                c.Close();
            toolStripProgressBar2.Value = toolStripProgressBar2.Minimum;
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            this.openDialog("solarNoon");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.openDialog("solarNoon");
        }

        private void get_TableNames()
        {
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataReader dr = null;

            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            catch (MySqlException exp)
            {
                MessageBox.Show("MySQL Error: " + exp.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string sql = "SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = 'phenomet' AND table_name LIKE '%std%'";
            
            try
            {
                cmd = new MySqlCommand(sql, conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string table = dr.GetString(0);
                    comboBox2.Items.Add(table);
                }
            }
            catch (MySqlException exp)
            {
                MessageBox.Show("MySQL Error: " + exp.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }           
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox3.Text = "Field";

            MySqlConnection c = null;
            MySqlDataReader dr = null;
            MySqlCommand columnNames = null;

            if (comboBox2.SelectedItem.ToString() == "dailysolarnoonvis")
            {
                comboBox4.Visible = true;
                comboBox5.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
            }
            else
            {
                comboBox4.Visible = false;
                comboBox5.Visible = false;
                label5.Visible = false;
                label6.Visible = false;
            }
            try
            {
                c = new MySqlConnection(connString);
                c.Open();
            }
            catch (MySqlException exp)
            {
                MessageBox.Show("MySQL Error: " + exp.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sql = "select distinct column_name from information_schema.columns where table_name='"+comboBox2.SelectedItem.ToString()+"'";
            try
            {
                columnNames = new MySqlCommand(sql, c);
                dr = columnNames.ExecuteReader();
                int col_count = 0;
                while (dr.Read())
                {
                    col_count++;
                    if (col_count <= 2)
                        continue;
                    if (comboBox2.SelectedItem.ToString() == "dailysolarnoonvis" && (col_count == 3 || col_count == 17 || col_count == 18))
                        continue;
                    if (comboBox2.SelectedItem.ToString() == "tw_std_clim" && (col_count == 20 || col_count == 21) )
                        continue;
                    if (comboBox2.SelectedItem.ToString() == "gi_std_clim" && (col_count == 29 || col_count == 30))
                        continue;
                    if (comboBox2.SelectedItem.ToString() == "p9_std_clim" && (col_count >= 29) )
                        continue;

                    string col = dr.GetString(0);
                    comboBox3.Items.Add(col);
                }
            }
            catch (MySqlException exp)
            {
                MessageBox.Show("MySQL Error: " + exp.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dr != null)
                    dr.Close();
                if (c.State == ConnectionState.Open)
                    c.Close();
                return;
            }
            if (dr != null)
                dr.Close();
            
            populate_date_range(c, comboBox2.SelectedItem.ToString(), sender);

            try
            {
                if (comboBox2.SelectedItem.ToString() == "dailysolarnoonvis")
                {
                    sql = "select DISTINCT CameraName from dailysolarnoonvis";

                    columnNames = new MySqlCommand(sql, c);
                    dr = columnNames.ExecuteReader();
                    
                    while (dr.Read())
                    {
                        string col = dr.GetString(0);
                        comboBox4.Items.Add(col);
                    }
                    if (dr != null)
                        dr.Close();

                    sql = "select DISTINCT ROI from dailysolarnoonvis";

                    columnNames = new MySqlCommand(sql, c);
                    dr = columnNames.ExecuteReader();
                    while (dr.Read())
                    {
                        string col = dr.GetString(0);
                        comboBox5.Items.Add(col);
                    }
                }

            }
            catch (MySqlException exp)
            {
                MessageBox.Show("MySQL Error: " + exp.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                if (c.State == ConnectionState.Open)
                    c.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string monthname3 = "January", monthname4 = "January";
            int year3 = 2000, year4 = 2040, day3 = 1, day4 = 31;

            if (cmbYear3.SelectedItem != null)
                year3 = Convert.ToInt32(cmbYear3.SelectedItem);
            if (cmbMonth3.SelectedItem != null)
                monthname3 = cmbMonth3.SelectedItem.ToString();
            int month3 = DateTime.ParseExact(monthname3, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
            if (cmbDay3.SelectedItem != null)
                day3 = Convert.ToInt32(cmbDay3.SelectedItem);
            DateTime start_date = new DateTime(year3, month3, day3);

            if (cmbYear4.SelectedItem != null)
                year4 = Convert.ToInt32(cmbYear4.SelectedItem);
            if (cmbMonth4.SelectedItem != null)
                monthname4 = cmbMonth4.SelectedItem.ToString();
            int month4 = DateTime.ParseExact(monthname4, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
            if (cmbDay4.SelectedItem != null)
                day4 = Convert.ToInt32(cmbDay4.SelectedItem);
            DateTime end_date = new DateTime(year4, month4, day4);
            
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Table has not been chosen!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (comboBox3.SelectedItem == null)
            {
                MessageBox.Show("Field has not been chosen!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            Form2 form2 = new Form2();
            if ((string)comboBox2.SelectedItem == "dailysolarnoonvis")
            {
                //form2.dataSetter((string)comboBox3.SelectedItem, "DATE", (string)comboBox2.SelectedItem, (string)comboBox4.SelectedItem, (string)comboBox5.SelectedItem, start_date, end_date);
                form2.FillChart("DATE", (string)comboBox3.SelectedItem, (string)comboBox2.SelectedItem, (string)comboBox4.SelectedItem, (string)comboBox5.SelectedItem, start_date, end_date);
                form2.Text = "Chart of " + (string)comboBox2.SelectedItem;
            }
            else if ((string)comboBox2.SelectedItem == "siteinfo")
            {
                //MessageBox.Show("SiteInfo table selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //form2.dataSetter((string)comboBox3.SelectedItem, "DATE", (string)comboBox2.SelectedItem, null, null, start_date, end_date);
                form2.FillChart("DATE", (string)comboBox3.SelectedItem, (string)comboBox2.SelectedItem, null, null, start_date, end_date);
                form2.Text = "Chart of " + (string)comboBox2.SelectedItem;
            }
            form2.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString().Equals("TWEE") || comboBox1.SelectedItem.ToString().Equals("GIBPE"))
            {
                panel2.Visible = true;
                label8.Visible = true;
                radioButton3.Visible = false;
            }
            else if(comboBox1.SelectedItem.ToString().Equals("PAS9"))
            {
                panel2.Visible = true;
                label8.Visible = true;
                radioButton3.Visible = true;
            }
            else
            {
                panel2.Visible = false;
                label8.Visible = false;
                radioButton3.Visible = false;
            }
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked == true)
                replaceOrignore = "INSERT IGNORE";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
                replaceOrignore = "REPLACE INTO";
        }

        private void GetData(string selectCommand)
        {
            MySqlDataAdapter dataAdapter = null;
            try
            {
                // Create a new data adapter based on the specified query.
                dataAdapter = new MySqlDataAdapter(selectCommand, connString);

                // Create a command builder to generate SQL update, insert, and
                // delete commands based on selectCommand. These are used to update the database.
                MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(dataAdapter);

                // Populate a new data table and bind it to the BindingSource.
                DataTable table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                
                if (table.Rows.Count <= 0)
                    MessageBox.Show("No matching data has been found.", "No Match", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    bindingSource1.DataSource = table;

                // Resize the DataGridView columns to fit the newly loaded content.
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private string get_table_name()
        {
            string table_name = "";

            if (comboBox6.SelectedItem.ToString() == "CRN")
                table_name = "cr_std_clim";
            else if (comboBox6.SelectedItem.ToString() == "TROM")
                table_name = "tr_std_clim";
            else if (comboBox6.SelectedItem.ToString() == "TWEE")
                table_name = "tw_std_clim";
            else if (comboBox6.SelectedItem.ToString() == "SCAN")
                table_name = "sc_std_clim";
            else if (comboBox6.SelectedItem.ToString() == "PAS9")
                table_name = "p9_std_clim";
            else if (comboBox6.SelectedItem.ToString() == "GIBPE")
                table_name = "gi_std_clim";

            return table_name;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox6.SelectedItem == null)
            {
                MessageBox.Show("Site has not been chosen!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            dataGridView1.DataSource = bindingSource1;
            string columns = "", Sql = "", table = "", monthname1 = "January", monthname2 = "January";
            int year1 = 2000, year2 = 2040, day1 = 1, day2 = 28;
            
            checkedListBox1.SetItemCheckState(0, CheckState.Checked);
                
            foreach (object itemChecked in checkedListBox1.CheckedItems)
            {
                columns = columns + itemChecked.ToString() + ",";
            }
            columns = columns.Remove(columns.LastIndexOf(','), 1);

            if (cmbYear1.SelectedItem != null)
                year1 = Convert.ToInt32(cmbYear1.SelectedItem);
            if(cmbMonth1.SelectedItem !=null )
                monthname1 = cmbMonth1.SelectedItem.ToString();
            int month1 = DateTime.ParseExact(monthname1, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
            if (cmbDay1.SelectedItem != null)
                day1 = Convert.ToInt32(cmbDay1.SelectedItem);
            DateTime start_date = new DateTime(year1, month1, day1);

            if (cmbYear2.SelectedItem != null)
                year2 = Convert.ToInt32(cmbYear2.SelectedItem);
            if (cmbMonth2.SelectedItem != null)
                monthname2 = cmbMonth2.SelectedItem.ToString();
            int month2 = DateTime.ParseExact(monthname2, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
            if (cmbDay2.SelectedItem != null)
                day2 = Convert.ToInt32(cmbDay2.SelectedItem);
            DateTime end_date = new DateTime(year2, month2, day2);

            table = get_table_name();
            
            Sql = "SELECT " + columns + " FROM " + table + " WHERE DATE BETWEEN '" + start_date.ToString("yyyy-MM-dd") + "' AND '" + end_date.ToString("yyyy-MM-dd") + "'";
            GetData(Sql);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            string table_name = get_table_name();
            string sql = "select column_name from information_schema.columns where table_name='" + table_name + "'";

            MySqlConnection conn = null;
            MySqlCommand cmd_columnNames = null;
            MySqlDataReader dr = null;

            populate_date_range(conn, table_name, sender);

            checkedListBox1.Items.Clear();
            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();

                cmd_columnNames = new MySqlCommand(sql, conn);
                dr = cmd_columnNames.ExecuteReader();

                while (dr.Read())
                {
                    string col = dr.GetString(0);
                    if (col.Contains("inserted_") || col.Contains("has_"))
                        continue;
                    checkedListBox1.Items.Add(col, false);
                }
            }
            catch (MySqlException exp)
            {
                MessageBox.Show("MySQL Error: " + exp.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
           
        }

        private void populate_date_range(MySqlConnection con, string table, object sender)
        {

            string sql = "SELECT MAX(DATE), MIN(DATE) FROM " + table;
            string ctrlName = ((ComboBox)sender).Name;
            
            DateTime maxDate = new DateTime(2040,1,1);
            DateTime minDate = new DateTime(2000,1,1);

            MySqlCommand cmd = null;
            MySqlDataReader dr = null;
            try
            {
                con = new MySqlConnection(connString);
                con.Open();

                cmd = new MySqlCommand(sql, con);
                dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();
                    if(!dr.IsDBNull(0))
                        maxDate = dr.GetDateTime(0);
                    if (!dr.IsDBNull(1))
                        minDate = dr.GetDateTime(1);
                }

                if (ctrlName.Equals("comboBox6"))
                {
                    cmbYear1.Items.Clear();
                    cmbYear2.Items.Clear();
                    for (int y = minDate.Year; y <= maxDate.Year; y++)
                    {
                        cmbYear1.Items.Add(y);
                        cmbYear2.Items.Add(y);
                    }
                }
                else
                {
                    cmbYear3.Items.Clear();
                    cmbYear4.Items.Clear();
                    for (int y = minDate.Year; y <= maxDate.Year; y++)
                    {
                        cmbYear3.Items.Add(y);
                        cmbYear4.Items.Add(y);
                    }
                }
                
            }
            catch (MySqlException exp)
            {
                MessageBox.Show("MySQL Error: " + exp.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
         
        }

        private void cmbMonth1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int year1 = 2000, month1 = 1;
            string monthname1 = "";

            cmbDay1.Items.Clear();

            if (cmbYear1.SelectedItem != null)
                year1 = Convert.ToInt32(cmbYear1.SelectedItem);
            if (cmbMonth1.SelectedItem != null)
                monthname1 = cmbMonth1.SelectedItem.ToString();
            if (!monthname1.Equals(""))
                month1 = DateTime.ParseExact(monthname1, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;

            for (int i = 1; i <= DateTime.DaysInMonth(year1, month1); i++)
            {
                cmbDay1.Items.Add(i.ToString());
            }
        }

        private void cmbMonth2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int year2 = 2040, month2 = 1;
            string monthname2 = "";

            cmbDay2.Items.Clear();

            if (cmbYear2.SelectedItem != null)
                year2 = Convert.ToInt32(cmbYear1.SelectedItem);
            if (cmbMonth2.SelectedItem != null)
                monthname2 = cmbMonth2.SelectedItem.ToString();
            if (!monthname2.Equals(""))
                month2 = DateTime.ParseExact(monthname2, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;

            for (int i = 1; i <= DateTime.DaysInMonth(year2, month2); i++)
            {
                cmbDay2.Items.Add(i.ToString());
            }

        }

        /*Update RH values of PAS9*/
        private void button9_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd1 = null, cmd2 = null;
            MySqlDataReader rdr = null;
            double TempDailyAvg, Svp, Vpd, VpdAvg;
            DateTime date;
            double[] datavalues = new double[4];
            
            int rows_count = 0;

            DataTable dt = new DataTable();
            dt.Columns.Add("Date",typeof(DateTime));
            dt.Columns.Add("RH_DAILY_MIN", typeof(double));
            dt.Columns.Add("RH_DAILY_MAX", typeof(double));
            dt.Columns.Add("RH_DAILY_MEAN", typeof(double));

            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            catch (MySqlException exp)
            {
                MessageBox.Show("MySQL Error: " + exp.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lockUnlockGUI("Lock");

            string get_sql = "SELECT Date, RH_DAILY_MIN, RH_DAILY_MAX, RH_DAILY_MEAN FROM gi_std_clim";

            try
            {
                cmd1 = new MySqlCommand(get_sql, conn);
                rdr = cmd1.ExecuteReader();
                
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        date = rdr.GetDateTime(0);
                        //Rhdailymin 
                        datavalues[0] = rdr.GetDouble(1);
                        //Rhdailymax 
                        datavalues[1] = rdr.GetDouble(2);
                        //Rhdailymean 
                        datavalues[2] = rdr.GetDouble(3);

                        dt.Rows.Add(date, datavalues[0], datavalues[1], datavalues[2]); 
                    }
                }
                if(rdr != null)
                    rdr.Close();

                toolStripProgressBar1.Step = 1;
                toolStripProgressBar1.Minimum = 0;
                toolStripProgressBar1.Maximum = dt.Rows.Count;

                foreach(DataRow row in dt.Rows)
                { 
                    date = Convert.ToDateTime(row["Date"]);
                    datavalues[0] = Convert.ToDouble(row["RH_DAILY_MIN"]);
                    datavalues[1] = Convert.ToDouble(row["RH_DAILY_MAX"]);
                    datavalues[2] = Convert.ToDouble(row["RH_DAILY_MEAN"]);

                    TempDailyAvg = getP9_tdailyAvg(date, conn);
                        
                    //Calculate VPD
                    if (TempDailyAvg != -99.99 && datavalues[2] != -99.99)
                    {
                        //VPD Formula, SVP (Pascals) = 610.7*10^(7.5T/(237.3+T))
                        Svp = 610.7 * (Math.Pow(10, (7.5 * TempDailyAvg) / (TempDailyAvg + 237.3)));
                        Vpd = (1 - (datavalues[2] / 100)) * Svp;
                        VpdAvg = Vpd / 1000;
                    }
                    else
                        VpdAvg = -99.99;

                    datavalues[3] = VpdAvg;

                    //bool exists = checkExistData(date, "p9_std_clim", conn);
                    bool has_giRHdata = false;
                    bool exists = checkIfDateExists(date, "p9_std_clim", "has_gi_rh_data", out has_giRHdata, conn);
                    
                    toolStripProgressBar1.PerformStep();

                    if (has_giRHdata && radioButton1.Checked)
                        continue;
                    
                    string put_sql = create_sql_of_StdP9(date, datavalues, exists, "P9_R", has_giRHdata);
                    
                    using(cmd2 = new MySqlCommand(put_sql, conn))
                    {
                        rows_count += cmd2.ExecuteNonQuery();
                    }
                       
                }
                  
            }
            catch (MySqlException exp)
            {
                MessageBox.Show("MySQL Error: " + exp.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (rdr != null)
                    rdr.Close();
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            MessageBox.Show(dt.Rows.Count + " rows found in the gi_std_clim table. " + rows_count + " rows updated in p9_std_clim table", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
            
            lockUnlockGUI("UnLock");
        }

        private double getP9_tdailyAvg(DateTime date, MySqlConnection con)
        {
            string sql = "SELECT T_DAILY_AVG FROM p9_std_clim WHERE date = '" + date.ToString("yyyy-MM-dd") + "'";
            double tdailyavg = -99.99;

            MySqlDataReader dr = null;
            
            try
            {
                MySqlCommand comd = new MySqlCommand(sql, con);
                dr = comd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    tdailyavg = dr.GetDouble(0);
                }
            }
            catch (MySqlException exp)
            {
                MessageBox.Show("MySQL Error: " + exp.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return tdailyavg;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("Nothing to Export", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl files (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "Export Excel File To";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = null;

                myStream = saveFileDialog.OpenFile();
                StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));

                string str = "";
                try
                {
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        if (i > 0)
                        {
                            str += "\t";
                        }
                        str += dataGridView1.Columns[i].HeaderText;
                    }

                    sw.WriteLine(str);

                    for (int j = 0; j < dataGridView1.Rows.Count; j++)
                    {
                        string tempStr = "";
                        for (int k = 0; k < dataGridView1.Columns.Count; k++)
                        {
                            if (k > 0)
                            {
                                tempStr += "\t";
                            }
                            if (dataGridView1.Rows[j].Cells[k].Value != null)
                                tempStr += dataGridView1.Rows[j].Cells[k].Value.ToString();
                        }
                        sw.WriteLine(tempStr);
                    }
                    MessageBox.Show("Data has been exported successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString(), "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                    if (myStream != null)
                        myStream.Close();
                }
            }
        }

        private void cmbMonth3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int year1 = 2000, month1 = 1;
            string monthname1 = "";

            cmbDay3.Items.Clear();

            if (cmbYear3.SelectedItem != null)
                year1 = Convert.ToInt32(cmbYear3.SelectedItem);
            if (cmbMonth3.SelectedItem != null)
                monthname1 = cmbMonth3.SelectedItem.ToString();
            if (!monthname1.Equals(""))
                month1 = DateTime.ParseExact(monthname1, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;

            for (int i = 1; i <= DateTime.DaysInMonth(year1, month1); i++)
            {
                cmbDay3.Items.Add(i.ToString());
            }
        }

        private void cmbMonth4_SelectedIndexChanged(object sender, EventArgs e)
        {
            int year1 = 2000, month1 = 1;
            string monthname1 = "";

            cmbDay4.Items.Clear();

            if (cmbYear4.SelectedItem != null)
                year1 = Convert.ToInt32(cmbYear4.SelectedItem);
            if (cmbMonth4.SelectedItem != null)
                monthname1 = cmbMonth4.SelectedItem.ToString();
            if (!monthname1.Equals(""))
                month1 = DateTime.ParseExact(monthname1, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;

            for (int i = 1; i <= DateTime.DaysInMonth(year1, month1); i++)
            {
                cmbDay4.Items.Add(i.ToString());
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked == true)
                S_replaceOrignore = "INSERT IGNORE";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked == true)
                S_replaceOrignore = "REPLACE INTO";
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 3)
            {
                //get_TableNames();
            }
        }
        

    }// end of class
}

