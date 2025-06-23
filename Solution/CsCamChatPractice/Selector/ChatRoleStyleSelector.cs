using CsCamChatPractice.Enum;
using CsCamChatPractice.Model;
using System.Windows;
using System.Windows.Controls;

namespace CsCamChatPractice.Selector
{
    public class ChatRoleStyleSelector : StyleSelector
    {
        public Style? UserRole
        { get; set; }

        public Style? OtherRole
        { get; set; }

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            if (UserRole is null
                || OtherRole is null)
            {
                return null;
            }

            if (item is not MsgModel model)
            {
                return null;
            }

            return (model.Role is ChatRole.You)
                ? UserRole : OtherRole;
        }
    }
}
