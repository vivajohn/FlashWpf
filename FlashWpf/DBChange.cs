using FlashCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashWpf
{
    public class DBChange
    {
        public static void To(DBNames targetDb)
        {
            Func<IDatabase> db;
            switch (targetDb)
            {
                case DBNames.Firebase:
                    db = () => ServiceLocator.GetInstance<IFirebase>();
                    break;
                default:
                    db = () => ServiceLocator.GetInstance<IAzure>();
                    break;
            }

            var ddb = ServiceLocator.GetInstance<IDynamicDB>();
            ddb.SetCurrentDB(db);
            ddb.Connect();
        }
    }
}
