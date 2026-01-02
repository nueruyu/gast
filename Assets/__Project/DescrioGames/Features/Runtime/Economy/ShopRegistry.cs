using UnityEngine;
using System.Collections.Generic;

namespace DescrioGames.Features.Economy
{
    public class ShopRegistry : MonoBehaviour
    {
        [SerializeField]
        Shop[] shops = { };

        public IReadOnlyList<Shop> Shops => shops;
    }
}