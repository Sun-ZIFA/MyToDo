using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common.Models
{
    /// <summary>
    /// 系统导航菜单实体类
    /// </summary>
    public class MenuBar : BindableBase
    {
        // 菜单图标
        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        //菜单名称
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        //菜单命名空间
        private string _nameSpace;
        public string NameSpace
        {
            get { return _nameSpace; }
            set { _nameSpace = value; }
        }

    }
}
