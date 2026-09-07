using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Core;

namespace SeungyungLib.FSM
{
    //컨디션 이름뒤에는 무조건 "ConditionSO"를 붙여야 한다.
    //그렇지 않으면 이름을 찾지 못해 ConditionSO에서 자동으로 타입을 할당해주지 못한다.
    public class ExampleConditionSO : ConditionSO
    {
        protected override ICondition OnCreate(IModuleOwner owner)
        {
            //내가 만들었던 AbstractCondition을 상속받은 커스텀 컨디션 클래스를 넣는다.
            return new ExampleCondition(owner, Type, IsNot);
        }
    }
}
