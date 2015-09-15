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


namespace example1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string executeRequest(string domain, string postData)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + domain + ".oncloud.gr/s1services") as HttpWebRequest;

            request.Method = "POST";

            // Create POST data and convert it to a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/json";

            // Set the ASuto Decompression property to GZip aand Deflate
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;


            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
           
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




        private void exec_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = executeRequest("demo", this.textBox1.Text);
        }
 
    }
 }
