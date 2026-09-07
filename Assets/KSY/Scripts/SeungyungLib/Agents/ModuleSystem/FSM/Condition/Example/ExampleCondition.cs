using SeungyungLib.FSM.Enum;
using SeungyungLib.ModuleSystem.Core;

namespace SeungyungLib.FSM
{
    public class ExampleCondition : AbstractCondition
    {
        //owner를 통해 조건 검사에 필요한 모듈을 가져온다.
        //isNot과 ConditionType, isNot은 필요하지 않다.
        public ExampleCondition(IModuleOwner owner, ConditionType type, bool isNot) : base(owner, type, isNot)
        {
        }

        protected override bool OnCheck()
        {
            //여기에 만들고 싶은 조건을 넣는다.
            return true;
        }
    }
}
