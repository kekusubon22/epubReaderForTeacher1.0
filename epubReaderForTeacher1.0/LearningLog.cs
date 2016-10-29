using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace epubReaderForTeacher1._0
{
    public class LearningLog
    {
        // メンバー変数の定義
        public string strokeId;
        public string behavior;

        //Getメソッドの定義
        public string GetStrokeId()
        {
            return strokeId;
        }

        public string GetBehavior()
        {
            return behavior;
        }

        //Setメソッドの定義
        public void SetStrokeId(string strokeId)
        {
            this.strokeId = strokeId;
        }

        public void SetBehavior(string behavior)
        {
            this.behavior = behavior;
        }
    }
}
