using HierInBreda.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierInBreda.Model
{
    /*
     * @author: Johannes [imp] Michel
     * @version: 1.0
     * @description: Model class Route containing a the sight IDs and wether they're yet visited or not.
     */
    public class Route
    {
        private Dictionary<Int32, Boolean> visited;
        private List<Sight> list;

        public Route(DataControl dc)
        {
            setList(dc);

            foreach(Sight sight in list)
            {
                visited.Add(sight.id, false);
            }
        }

        private async void setList(DataControl dc)
        {
            list = await dc.getSight();
        }

        /// <summary>
        /// Returns false if the sight hasn't been visited and true if it was visited before.
        /// </summary>
        public bool isVisited(int ID)
        {
            return visited[ID];
        }

        /// <summary>
        /// Sets a sight ID to the boolean specified.
        /// </summary>
        public void setVisited(int ID, Boolean seen)
        {
            visited[ID] = seen;
        }
    }
}
