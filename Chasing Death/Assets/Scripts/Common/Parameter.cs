using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Common {
    public class Parameter {
        static Parameter prm;

        private Parameter () { }

        public static Parameter Prm () {
            if (prm == null)
                prm = new Parameter ();
            return prm;
        }

    }
}
