using System.Collections.Generic;
using System.Linq;
using Railway.Shop.Data;
using TGS;
using UnityEngine;
using Zenject;

namespace Railway.Gameplay
{
    public class NeighborFinder
    {
        [Inject]
        private TerrainGridSystem _tgs;
        
        public Cell[] GetAvailableCell(ShopItem currentItem, Cell startCell,
            Dictionary<ItemType, List<Cell>> _occupiedCells)
        {
            Cell[] availableCells = new Cell[15];

            if (!_occupiedCells.TryGetValue(currentItem.ItemType.ItemType, out var cells))
            {
                _occupiedCells[currentItem.ItemType.ItemType] = new List<Cell>();
                cells = _occupiedCells[currentItem.ItemType.ItemType];
            }

            switch (currentItem.ItemType.ItemType)
            {
                case ItemType.Locomotive:
                    availableCells = _tgs.Cells
                        .Where(x => _tgs.CellGetTag(_tgs.CellGetIndex(x)) == (int)CellBuildingType.RAILS).ToArray();
                    break;
                case ItemType.Workers:
                    availableCells = _tgs.Cells
                        .Where(x => _tgs.CellGetTag(_tgs.CellGetIndex(x)) == (int)CellBuildingType.BUILD).ToArray();
                    break;
                case ItemType.Carriage:
                    availableCells = _tgs.Cells
                        .Where(x => _tgs.CellGetTag(_tgs.CellGetIndex(x)) == (int)CellBuildingType.RAILS).ToArray();
                    break;
                default:
                    availableCells = _tgs.CellGetNeighbours(startCell).ToArray();
                    break;
            }
            

            HighlightAvailableCells(availableCells);

            return availableCells;
        }

        private void HighlightAvailableCells(Cell[] availableCells)
        {
            foreach (var cell in availableCells)
            {
                int index = _tgs.CellGetIndex(cell);
                _tgs.CellToggleRegionSurface(index, true, Color.green);
            }
        }

        public void ClearHighlightedCells(Cell[] availableCells)
        {
            foreach (var cell in availableCells)
            {
                int index = _tgs.CellGetIndex(cell);
                _tgs.CellToggleRegionSurface(index, false, Color.green);
            }
        }
    }
}