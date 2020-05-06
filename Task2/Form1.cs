using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.ToolTips;
using GMap.NET.WindowsForms.Markers;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using Services;
using Business_Objects;

namespace Task2
{
    public partial class Form1 : Form
    {
     

        public Service service = new Service();

        public Form1()
        {
            InitializeComponent();            
        }       

        private void gMapControl1_Load(object sender, EventArgs e)
        {            
            filesComboBox.DataSource = service.GetFileNames();

            #region MAPSETTINGS
            gMapControl1.Bearing = 0;
            gMapControl1.CanDragMap = true; //Открываем доступ к манипулированию картой мышью через зажатие правой кнопкой(по умолчанию).
            gMapControl1.DragButton = MouseButtons.Left; //Меняем кнопку манипулирования с правой кнопки(по умолчанию) на левую кнопку мыши.
            gMapControl1.GrayScaleMode = true;
            gMapControl1.MaxZoom = 18; //Устанавливаем максимальное приближение.
            gMapControl1.MinZoom = 2; //Устанавливаем минимальное приближение.
            gMapControl1.Zoom = 4;
            gMapControl1.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter; //Устанавливаем центр приближения/удаления курсор мыши.

            gMapControl1.PolygonsEnabled = true; //Открываем отображение полигонов на карте.
            gMapControl1.MarkersEnabled = true; //Открываем отображение маркеров на карте.
            gMapControl1.NegativeMode = false; //Отказываемся от негативного режима
            gMapControl1.ShowTileGridLines = false; //Скрываем внешнюю сетку карты с заголовками
            
            gMapControl1.Dock = DockStyle.None; //Закрепляем контрол внутри формы, чтобы размеры контрола изменялись вместе с размером формы.
            gMapControl1.MapProvider = GMapProviders.GoogleMap; //Указываем что будут использоваться карты OpenStreetMaps. Здесь куча карт на выбор.
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            #endregion

            gMapControl1.Position = new PointLatLng(40, -100); 

            AllFuncs("D:/TwitTrends/Data/" + filesComboBox.SelectedItem);            

        }       

        private void AllFuncs(string fileName)
        {
            //sentiments
            Dictionary<string, double> sents =service.GetSentiments();
            //tweets
            List<Tweet> tweets = new List<Tweet>();
            tweets = service.ParseTweets(fileName,sents);
            //states
            Dictionary<string, State> states = service.GetStates();
            service.FillStates(tweets);

            List<string> keys = new List<string>(states.Keys);
            List<PointLatLng> points = new List<PointLatLng>();
            
           
            for (int i = 0; i < keys.Count; i++)
            {

                for (int j = 0; j < states[keys[i]].Polygons.Count; j++)
                {
                    foreach (List<double> item in states[keys[i]].Polygons[j])
                    {
                        points.Add(new PointLatLng(item[1], item[0]));
                    }
                    if (states[keys[i]].Weight > 0)
                    {
                        int addColor = 255 - Convert.ToInt32(states[keys[i]].Weight * 500);
                        if (addColor < 0) { addColor = 128; }
                        Color color = Color.FromArgb(200, 255, addColor, 0);
                        DrawPolygon(points, keys[i], color);
                    }
                    if (states[keys[i]].Weight == 0 && states[keys[i]].StateTweets.Count != 0)
                    {
                        Color color = Color.FromArgb(200, 255, 255, 255);
                        DrawPolygon(points, keys[i], color);
                    }
                    if (states[keys[i]].Weight < 0)
                    {
                        int addColor = 255 + Convert.ToInt32(states[keys[i]].Weight * 500);
                        if (addColor < 0) { addColor = 128; }
                        Color color = Color.FromArgb(200, 0, 0, addColor);
                        DrawPolygon(points, keys[i], color);
                    }
                    if (states[keys[i]].StateTweets.Count == 0)
                    {
                        Color color = Color.FromArgb(200, 125, 125, 125);
                        DrawPolygon(points, keys[i], color);

                    }
                    points.Clear();
                }

            }

            for (int i = 0; i < tweets.Count; i++)
            {
                if (tweets[i].Weight > 0)
                {
                    DrawMarker(tweets[i].Point, tweets[i].Message,GMarkerGoogleType.yellow_small);
                }
                if(tweets[i].Weight < 0)
                {
                    DrawMarker(tweets[i].Point, tweets[i].Message, GMarkerGoogleType.blue_small);
                }
                if(tweets[i].Weight == 0)
                {
                    DrawMarker(tweets[i].Point, tweets[i].Message, GMarkerGoogleType.white_small);
                }
            }

        }

        private void DrawPolygon(List<PointLatLng> points,string state,Color color)
        {
            GMapOverlay overlay = new GMapOverlay("polygon");           
            GMapPolygon mapPolygon = new GMapPolygon(points, state); 
            mapPolygon.Tag = state;
            mapPolygon.Fill = new SolidBrush(color);             
            mapPolygon.Stroke = new Pen(Color.Red, 1); 
            overlay.Polygons.Add(mapPolygon);
            gMapControl1.Overlays.Add(overlay); 
        }

        private void DrawMarker(PointLatLng point,string message,GMarkerGoogleType gMarker)
        {
            GMapOverlay markersOverlay = new GMapOverlay("marker");       
            GMapMarker marker = new GMarkerGoogle(point, gMarker);          
            marker.ToolTip = new GMapRoundedToolTip(marker); 
            marker.ToolTipText = message; 
            markersOverlay.Markers.Add(marker); 
            gMapControl1.Overlays.Add(markersOverlay); 
            
        }

        private void filesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            gMapControl1.Overlays.Clear();
            gMapControl1.Refresh();
            AllFuncs("D:/TwitTrends/Data/" + filesComboBox.SelectedItem);
            gMapControl1.Zoom += 0.5;
            gMapControl1.Zoom -= 0.5;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(gMapControl1.MarkersEnabled == true)
            {
                gMapControl1.MarkersEnabled = false;
            }
            else
            {
                gMapControl1.MarkersEnabled = true;                
            }
            gMapControl1.Refresh();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (gMapControl1.PolygonsEnabled == true)
            {
                gMapControl1.PolygonsEnabled = false;
            }
            else
            {
                gMapControl1.PolygonsEnabled = true;
            }
            gMapControl1.Refresh();
        }

       
    }
}
