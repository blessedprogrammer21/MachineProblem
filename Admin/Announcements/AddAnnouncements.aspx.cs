﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Machine_Problem.master
{
    public partial class WebForm9 : System.Web.UI.Page
    {
        public class Photos
        {
            public List<string> paths { get; set; }
            public List<string> fileNames { get; set; }
        }

        protected void btnAddAnnouncement_Click(object sender, EventArgs e)
        {
            insertPhotos(insertAnnouncement());
            ScriptManager.RegisterStartupScript(this, this.GetType(),
                "redirect", "alert('New Announcement Posted.'); window.location='" +
                Request.ApplicationPath + "Admin/Announcements/ViewAnnouncements.aspx';", true);
        }

        protected int insertAnnouncement()
        {
            string sqlCommand = "INSERT INTO Announcements VALUES (@announceTitle , @announceText , GETDATE());";

            using (SqlConnection connection = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(sqlCommand, connection);
                connection.Open();

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("announceTitle", txtAnnounceTitle.Text);
                command.Parameters.AddWithValue("announceText", txtAnnounceText.Text);
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.CommandText = "SELECT @@IDENTITY";
                int announceID = Convert.ToInt32(command.ExecuteScalar());

                return announceID;
            }
        }

        protected void insertPhotos(int announceID)
        {
            Photos photos = getPhotos();

            if (photos.paths.Count > 0)
            {
                string sqlCommand = "INSERT INTO AnnouncementPhotos VALUES (@announceID, @photoName, @photoPath);";

                using (SqlConnection connection = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
                {
                    connection.Open();

                    for (int i = 0; i < photos.paths.Count(); i++)
                    {
                        SqlCommand command = new SqlCommand(sqlCommand, connection);

                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("announceID", announceID);
                        command.Parameters.AddWithValue("photoName", photos.fileNames[i]);
                        command.Parameters.AddWithValue("photoPath", photos.paths[i]);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        protected Photos getPhotos()
        {
            List<string> paths = new List<string>();
            List<string> fileNames = new List<string>();

            if (filePhoto.HasFile)
            {
                foreach (HttpPostedFile postedFile in filePhoto.PostedFiles)
                {
                    if (postedFile.ContentType.Contains("image"))
                    {
                        string fileName = Path.GetFileName(postedFile.FileName);
                        Guid g = Guid.NewGuid();
                        postedFile.SaveAs(Server.MapPath("Photos/") + g + fileName);
                        paths.Add("Photos/" + g + fileName);
                        fileNames.Add(fileName);
                    }
                }
            }
            return new Photos { paths = paths, fileNames = fileNames };
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewAnnouncements.aspx");
        }

    }
}