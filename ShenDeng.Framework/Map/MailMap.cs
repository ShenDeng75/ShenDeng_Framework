using ShenDeng.Framework.Domain;
using ShenDeng.Framework.Base;

namespace ShenDeng.Framework.Maps
{
    public class MailMap : BaseClassMap<Mail>
    {
        public MailMap()
        {
            Map(x => x.Name);
            Map(x => x.MailAddre);
            Map(x => x.Duty);
            Map(x => x.PassWord);
        }
    }
}
