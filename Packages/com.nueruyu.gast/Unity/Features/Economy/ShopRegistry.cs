using System.Linq;
using UnityEngine;

namespace Gast.Unity.Features.Economy
{
    public class ShopRegistry : MonoBehaviour
    {
        [SerializeField]
        Shop[] shops = { };

        public Shop[] GetShops()
        {
            return shops
                .Where(x => x != null && x.isActiveAndEnabled)
                .ToArray();
        }
    }
}