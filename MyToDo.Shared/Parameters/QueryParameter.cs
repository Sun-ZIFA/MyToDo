using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Shared.Parameters
{
    public class QueryParameter
    {
        /// <summary>
        /// 当前查询第几页
        /// </summary>
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// 查询多少数量
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 通用条件
        /// </summary>
        public string Search { get; set; }
    }
}
