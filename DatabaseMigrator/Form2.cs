using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Configuration;

namespace DatabaseMigratior
{
    public partial class Form2 : Form
    {
        internal class Item
        {
            public DateTime X { get; private set; }
            public double Y { get; private set; }
            public Item(DateTime x, double y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        string connString; 
    
        public Form2()
        {
            InitializeComponent();
            //this.chart1.MouseMove += new MouseEventHandler(chart1_MouseMove);
            this.tooltip.AutomaticDelay = 10;

            GetConnectionStringByName();
        }

        private void GetConnectionStringByName()
        {
            // Look for the name in the connectionStrings section.
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["phenologyDBConnection"]; //phenologyLocalDBConnection

            // If found, return the connection string. 
            if (settings != null)
                connString = settings.ConnectionString;
        }
       
        public void FillChart(string xVal, string yVal, string tableNameVal,string cameraname,string roi, DateTime stdate, DateTime endate)
        {
            this.chart1.Series.Clear();

            var seriesLines = this.chart1.Series.Add("Line");
            seriesLines.ChartType = SeriesChartType.Line; //System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            seriesLines.XValueMember = "X";
            seriesLines.YValueMembers = "Y";
            seriesLines.Color = Color.Red;

            var seriesPoints = this.chart1.Series.Add("Points");
            seriesPoints.ChartType = SeriesChartType.Point; //System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            seriesPoints.XValueMember = "X";
            seriesPoints.YValueMembers = "Y";
            
            //string connString = "server=localhost; user id=root; password= Jrn.phenomet;database=phenomet; pooling=false";
            string x, y, tableName;
            x = xVal;
            y = yVal;
            tableName = tableNameVal;
            chart1.ChartAreas[0].AxisX.Title = x;
            chart1.ChartAreas[0].AxisY.Title = y;
            //-------------------------
            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorX.IntervalType = DateTimeIntervalType.Auto;
            chart1.ChartAreas[0].CursorX.Interval = 1;
            //--------------------------
            List<Item> items = new List<Item>();
 
            MySqlConnection c = null;
            MySqlDataReader dr = null;

            try
            {
                c = new MySqlConnection(connString);
                c.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string sql = "";
            if (cameraname == null || roi == null)
            {
                sql = "select " + x + "," + y + " from " + tableName + " WHERE Date BETWEEN '" + stdate.ToString("yyyy-MM-dd") 
                    + "' AND '" + endate.ToString("yyyy-MM-dd") + "'"+ " ORDER BY " + x; 
            }
            else
            {
                sql = "select " + x + "," + y + " from " + tableName + " WHERE CameraName='" + cameraname + "' AND ROI='" + roi + "'" 
                    + " AND Date BETWEEN '" + stdate.ToString("yyyy-MM-dd") + "' AND '" + endate.ToString("yyyy-MM-dd") + "'" + " ORDER BY " + x;
            }
            try
            {
                MySqlCommand columnNames = new MySqlCommand(sql, c);
                dr = columnNames.ExecuteReader();
                double vy;
                while (dr.Read())
                {
                    if (!dr.IsDBNull(1))
                        vy = dr.GetDouble(1);
                    else
                        vy = -99.99;

                    DateTime dx = dr.GetDateTime(0);
                    items.Add(new Item(dx, vy));
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("MySQL Error: " + e.ToString(), "Database ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            { 
                if (dr != null)
                    dr.Close();
                if (c.State == ConnectionState.Open)
                    c.Close();
            }

            this.chart1.DataSource = items;
        }

        Point? prevPosition = null;
        ToolTip tooltip = new ToolTip();

        void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            if (prevPosition.HasValue && pos == prevPosition.Value)
                return;
            tooltip.RemoveAll();
            prevPosition = pos;
            var results = chart1.HitTest(pos.X, pos.Y, false,
                                            ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    var prop = result.Object as DataPoint;
                    if (prop != null)
                    {
                        var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                        var pointYPixel = result.ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);

                        // check if the cursor is really close to the point (2 pixels around)
                        if (Math.Abs(pos.X - pointXPixel) < 2 &&
                            Math.Abs(pos.Y - pointYPixel) < 2)
                        {
                            //tooltip.Show("X=" + prop.XValue + ", Y=" + prop.YValues[0], this.chart1, pos.X, pos.Y - 15);
                            DateTime d = System.DateTime.FromOADate(prop.XValue);
                            string sd = d.ToString("MM-dd-yyyy");
                            tooltip.Show("X=" + sd + ", Y=" + prop.YValues[0], this.chart1, pos.X, pos.Y - 15);
                        }
                    }
                }
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
