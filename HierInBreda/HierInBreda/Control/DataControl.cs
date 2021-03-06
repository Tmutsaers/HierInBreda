﻿using HierInBreda.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HierInBreda.Control
{
        /*
     * @author:Raymond Rohder
     * @version: 2.0
     * @description: Class that controls the database
     */
    public class DataControl
    {
        SQLiteAsyncConnection conn;
        private List<Sight> list;

        public DataControl()
        {
            list = new List<Sight>();
            
            ConnData();
            InitData();
        }

        private void ConnData()
        {
            //Creates database if it doesn't exists
            var dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");

            conn = new SQLite.SQLiteAsyncConnection(dbPath);
        }

        private async void InitData()
        {
            //verwijder table
            //var result1 = await conn.DropTableAsync<Sight>();

            //Create table
            var result = await conn.CreateTableAsync<Sight>();

            //Delete
             //await conn.ExecuteAsync("DELETE FROM Sight", new object[] { });
                     

            int count = await conn.ExecuteScalarAsync<int>("Select Count(id) From Sight");

            //if database is empty -> fill it
            if(count < 1)
            {
                //adding a list of sights
                XDocument doc = XDocument.Load("Sights.xml");
                var sights = from elm in doc.Descendants("sight")
                         select new Sight
                         {
                             id = (int)elm.Attribute("id"),
                             name = (string)elm.Element("name"),
                             lat = (string)elm.Element("latitude"),
                             longi = (string)elm.Element("longitude"),
                             img = (string)elm.Element("image"),
                             audio = (string)elm.Element("audio"),
                             disc = (string)elm.Element("description"),
                             discEng = (string)elm.Element("descriptionEng")
                         };
            List<Sight> sightsList = sights.ToList<Sight>();

            await conn.InsertAllAsync(sightsList);

            var tableQuery = conn.Table<Sight>();        
            }

            //Select all query
            //List<Sight> re1 = await tableQuery.ToListAsync();
        
            //Of select query. Scalar returns always 1 row
            //String i = await conn.ExecuteScalarAsync<String>("Select disc From Sight WHERE name = ?", new object[] { "Valkenberg" });

            //Query returns a list
            //List<Sight> e = await conn.QueryAsync<Sight>("Select * From Sight WHERE id = ? OR id = ?", new object[] { "5", "12" });
        }
                
        public async Task<List<Sight>> getSight()
        {
            list = await conn.QueryAsync<Sight>("Select * From Sight");
            return list;
        }
    }
}
