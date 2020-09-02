using System;
using TechTalk.SpecFlow;

namespace DevBoost.DroneDelivery.Test.BDD.Drone
{
    [Binding]
    public class Drone_AdicionarUmNovoDroneSteps
    {
        [Given(@"Que eu possua um drone")]
        public void DadoQueEuPossuaUmDrone()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"O Usuario esteja logado")]
        public void DadoOUsuarioEstejaLogado()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"O usuario adicionar um drone")]
        public void QuandoOUsuarioAdicionarUmDrone()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"O drone será cadastrado")]
        public void EntaoODroneSeraCadastrado()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
