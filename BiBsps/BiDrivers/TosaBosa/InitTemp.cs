using System.Collections.Generic;
using System.Linq;
using BiBsps.BiGlobalFiies;

namespace BiBsps.BiDrivers.TosaBosa
{
    public class InitTemp
    {
        private readonly string _tempFile;
        private List<BoardCurrents> _boardCurrents;
        public InitTemp(string strFilePath)
        {
            _tempFile = strFilePath;
            LoadCalData();
        }
        private void LoadCalData()
        {
            Utility.Load(_tempFile, out _boardCurrents);
            if (_boardCurrents == null)
                _boardCurrents = new List<BoardCurrents>();
        }
        public void SaveCalData(BoardCurrents current)
        {
            var boardName = current.BoardName;
            var result = from x in _boardCurrents where x.BoardName == boardName select x;
            var boardCurrentses = result as BoardCurrents[] ?? result.ToArray();
            var curs = boardCurrentses.Any() ? boardCurrentses.ToArray() : null;
            if (curs != null)
            {
                foreach (var cur in curs)
                    _boardCurrents.Remove(cur);
            }
            _boardCurrents.Add(current);
            Utility.Dump(_tempFile, _boardCurrents);
        }
        public bool GetCalData(string boardName,int seat, out SeatInit initInfo)
        {
            initInfo = null;
            var result = from x in _boardCurrents where x.BoardName == boardName select x;
            var boardCurrentses = result as BoardCurrents[] ?? result.ToArray();
            var info = boardCurrentses.Any() ? boardCurrentses.First() : null;
            if (info == null)
            {
                return false;
            }
            else
            {
                var res = from x in info.SlotInit where x.Position == seat select x;
                var seatInits = res as SeatInit[] ?? res.ToArray();
                if (seatInits.Any())
                {
                    initInfo = seatInits.First();
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }

        public bool RemoveCalData(string boardName, int seat)
        {
            var result = from x in _boardCurrents where x.BoardName == boardName select x;
            var boardCurrentses = result as BoardCurrents[] ?? result.ToArray();
            var info = boardCurrentses.Any() ? boardCurrentses.First() : null;

            if (info == null) return false;
            var res = from x in info.SlotInit where x.Position == seat select x;
            var seatInits = res as SeatInit[] ?? res.ToArray();
            if (seatInits.Any())
            {
                var data = seatInits.First();
                info.SlotInit.Remove(data);
                Utility.Dump(_tempFile, _boardCurrents);
                return true;
            }
            else
            {
                return false;
            }
            

        }

        public bool IsCalDataExist(string boardName,int seat)
        {
            var result = from x in _boardCurrents where x.BoardName == boardName select x;
            var boardCurrentses = result as BoardCurrents[] ?? result.ToArray();
            var info = boardCurrentses.Any() ? boardCurrentses.First() : null;
            if (info == null) return false;
            var res = from x in info.SlotInit where x.Position == seat select x;
            var seatInits = res as SeatInit[] ?? res.ToArray();
            return seatInits.Any();
        }

    }
}
