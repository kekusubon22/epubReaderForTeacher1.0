using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace epubReader4._0_Dino
{
    public class User
    {
        // メンバー変数の定義
        public string id;
        public string type;

        //Getメソッドの定義
        public string GetId()
        {
            return id;
        }

        public string GetType()
        {
            return type;
        }

        //Setメソッドの定義
        public void SetId(string id)
        {
            this.id = id;
        }

        public void SetType(string type)
        {
            this.type = type;
        }
    }
}
