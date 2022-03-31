using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common.Models
{
    public class BaseDto
    {
        private int id;
        private DateTime createDate;
        private DateTime updateDate;

        /// <summary>
        /// Id
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate
        {
            get { return updateDate; }
            set { updateDate = value; }
        }


    }
}
