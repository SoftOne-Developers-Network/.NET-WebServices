//
//  Created by Giannis Giorgoulakis on 7/7/13.
//  Copyright (c) 2013 SoftOne Technologies. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;


namespace example2
{
    public partial class Form1 : Form
    {
        static string clientID ="";

        public Form1()
        {
            InitializeComponent();
        }

        private string executeRequest(string postData)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://demo.oncloud.gr/s1services") as HttpWebRequest;

            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            //request.ContentType = "application/json";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            //
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.

            //-------------------------------------------------------------------
            string responseFromServer = "";
            Encoding Soft1Encoding = Encoding.GetEncoding(1253);

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream, Soft1Encoding))
                    {
                        responseFromServer = reader.ReadToEnd();
                    }
                }
            }
            return responseFromServer;
        }

        private string Login(string username, string password)
        {
            string loginStr = "\"service\": \"login\",  \"username\": \"{0}\",  \"password\":\"{1}\",  \"appId\": \"157\"";
            string rs = executeRequest("{" + string.Format(loginStr, username, password) + "}");

            Newtonsoft.Json.Linq.JObject jo = Newtonsoft.Json.Linq.JObject.Parse(rs);
            Boolean success = Convert.ToBoolean(jo["success"].ToString());
            if (success) {
                return jo["clientID"].ToString();
            } else {
                return "";
            }
        }


        private string authenticate(string theClientId)
        {
            string authStr = "\"service\": \"authenticate\",\"clientID\": \"{0}\", \"COMPANY\": \"1000\", \"BRANCH\": \"1000\", \"MODULE\": \"0\", \"REFID\": \"411\", \"USERID\": \"1\"";
            string rs = executeRequest("{" + string.Format(authStr, theClientId) + "}");

            Newtonsoft.Json.Linq.JObject jo = Newtonsoft.Json.Linq.JObject.Parse(rs);
            Boolean success = Convert.ToBoolean(jo["success"].ToString());
            if (success)
            {
                return jo["clientID"].ToString();
            }
            else
            {
                MessageBox.Show(jo["error"].ToString());
                return "";
            }
        }



        private string postNewSalDoc(string seriesId, string trdrId, string itemId)
        {
            string postStr = "{\"OBJECT\": \"saldoc\",\"SERVICE\": \"setData\",\"clientID\": \"" + clientID +
                "\",\"appId\": \"157\",\"KEY\": \"\",\"data\": {\"SALDOC\": [{\"SERIES\": \"" + seriesId + 
                "\",\"TRDR\": \""+trdrId+
                "\",\"TRNDATE\": \"2015-09-20 12:00:00\"}],\"ITELINES\": [{\"LINENUM\": \"0\",\"MTRL\": \"" + itemId +
                "\",\"QTY1\": \"1\"}]}}";
            string rs = executeRequest(postStr);

            Newtonsoft.Json.Linq.JObject jo = Newtonsoft.Json.Linq.JObject.Parse(rs);
            Boolean success = Convert.ToBoolean(jo["success"].ToString());
            if (success)
            {
                MessageBox.Show("Success post");
                return jo["id"].ToString();
            }
            else
            {
                MessageBox.Show(jo["error"].ToString());
                return "0";
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            string tempClientID = Login(textBox1.Text, textBox2.Text);
            if (tempClientID != "")
            {
                clientID = authenticate(tempClientID);
                MessageBox.Show("Success login");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (clientID != "")
            {
                textBox4.Text = execSql(textBox3.Text);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox4.Text = postNewSalDoc(series.Text, trdr.Text, item.Text);
        }


    }
}
