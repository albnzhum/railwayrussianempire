using Railway.Gameplay.UI;
using Railway.Shop.Data;
using TGS;
using UnityEngine;
using Zenject;

namespace Railway.Gameplay
{
    public class PlacementManager : MonoBehaviour
    {
        private Wagon previousWagon;

        private bool _isFirstRail = false;
        
        [Inject]
        private TerrainGridSystem _tgs;

        public void PlaceItem(Cell startCell, Cell currentCell, ItemData _itemData)
        {
            ResourcesManager.Instance.Spend(ResourceType.Gold, _itemData.CurrentItem.Price);

            var cellPosition = _tgs.CellGetPosition(currentCell);

            _itemData.CurrentPlacer.Place(currentCell);

            switch (_itemData.CurrentItem.ItemType.ItemType)
            {
                case ItemType.Rails:

                    if (!_isFirstRail)
                    {
                        Vector3 position = _tgs.CellGetPosition(startCell);
                        RailBuilder.Instance.Build(position);

                        _isFirstRail = true;
                    }

                    RailBuilder.Instance.Build(cellPosition);
                    break;

                case ItemType.Locomotive:
                case ItemType.Carriage:

                    AddWagonToTrain(_itemData.CurrentPlacer.Prefab.GetComponentInChildren<Wagon>());
                    break;
            }
        }
        
        private void AddWagonToTrain(Wagon newWagon)
        {
            if (previousWagon == null)
            {
                previousWagon = newWagon;
            }
            else
            {
                previousWagon.back = newWagon;

                if (!newWagon.isEngine)
                {
                    newWagon.front = previousWagon;
                    newWagon.SetupRecursively(previousWagon);
                }
                else
                {
                    newWagon.SetupRecursively(null);
                }

                previousWagon = newWagon;
            }
        }
    }
}