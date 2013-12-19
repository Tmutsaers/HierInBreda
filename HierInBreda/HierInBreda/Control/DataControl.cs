using HierInBreda.Model;
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
     * @version: 1.3
     * @description: Class that controls the database
     */
    public class DataControl
    {
        SQLiteAsyncConnection conn;

        public DataControl()
        {
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
            //Create table
            var result = await conn.CreateTableAsync<Sight>();

            //Delete
            // await conn.ExecuteAsync("DELETE FROM Sight", new object[] { });

            //adding a list of sights
            XDocument doc = XDocument.Load("Sights.xml");

            int count = 2;// await conn.ExecuteScalarAsync<int>("Select Count id From Sight");

            //if database is empty -> fill it
            if(count < 1)
            { 
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

        //returns a list with Dutch description
        public async Task<List<Sight>> getSightDutch()
        {
            List<Sight> list = await conn.QueryAsync<Sight>("Select id, name, latitude, longitude, image, audio, description From Sight");
            return list;
        }

        //returns a list with Englisch description
        public async Task<List<Sight>> getSightEng()
        {
            List<Sight> list = await conn.QueryAsync<Sight>("Select id, name, latitude, longitude image, audio, descriptionEng From Sight");
            return list;
        }
    }
}
