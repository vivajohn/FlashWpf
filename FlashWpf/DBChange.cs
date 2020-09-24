using FlashCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashWpf
{
    public enum DBNames
    {
        Azure,
        Firebase
    }

    public class DBChange
    {

        public static void To(DBNames targetDb)
        {
            IDatabase db;
            if (targetDb == DBNames.Firebase)
            {
                db = ServiceLocator.GetInstance<IFirebase>();
            }
            else
            {
                db = ServiceLocator.GetInstance<IAzure>();
            }

            var ddb = ServiceLocator.GetInstance<IDynamicDB>();
            ddb.SetCurrentDB(db);
        }
    }
}
