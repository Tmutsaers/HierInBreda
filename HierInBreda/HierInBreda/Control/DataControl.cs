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
     * @description:Klasse die de database beheert
     */
    class DataControl
    {
        SQLiteAsyncConnection conn;

        public DataControl()
        {
            ConnData();
            InitData();
        }

        private void ConnData()
        {
            //Als de database niet bestaat maakt hij die automatisch aan.
            var dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");

            conn = new SQLite.SQLiteAsyncConnection(dbPath);
        }

        private async void InitData()
        {
            //Tabel aanmaken. Dit kan natuurlijk ook via sql. Zie delete hieronder.
            var result = await conn.CreateTableAsync<Sight>();

            //Delete kan via methode maak ook via sql.
            // await conn.ExecuteAsync("DELETE FROM Sight", new object[] { });

            //Makkleijke manier om sights toe te voegen. Via een lijst van objecten. Kan ook natuurlijk via sql met execute
            XDocument doc = XDocument.Load("Sights.xml");

            int count = await conn.ExecuteScalarAsync<int>("Select Count id From Sight");

            //kijkt of er iets in de database staat
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
        
            //Of select query. Scalar return altijd 1 rij
            //String i = await conn.ExecuteScalarAsync<String>("Select disc From Sight WHERE name = ?", new object[] { "Valkenberg" });

            //Query return lijst met uitkomsten.
            //List<Sight> e = await conn.QueryAsync<Sight>("Select * From Sight WHERE id = ? OR id = ?", new object[] { "5", "12" });
        }

        public async Task<List<Sight>> getSightDutch()
        {
            List<Sight> list = await conn.QueryAsync<Sight>("Select id, name, latitude, longitude, image, audio, description From Sight");
            return list;
        }

        public async Task<List<Sight>> getSightEng()
        {
            List<Sight> list = await conn.QueryAsync<Sight>("Select id, name, latitude, longitude image, audio, descriptionEng From Sight");
            return list;
        }
    }
}
