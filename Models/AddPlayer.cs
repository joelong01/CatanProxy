using System;
using System.Collections.Generic;
using System.Text;

namespace Catan.Proxy
{
    /// <summary>
    ///     This class has all the data associated with adding a player to the game
    /// </summary>
    public class AddPlayerModel : LogHeader
    {
        public List<string> PlayerNames { get; set; } = null;
        public AddPlayerModel() : base()
        {
            Action = CatanAction.AddPlayer;
        }

    }
}
