using UnityEngine;

// Acumula o que o jogador vai montando enquanto carrega a xícara pelos
// balcões. Cada equipamento (máquina de espresso, bases de chá/matcha, água,
// leite, vaporizador, sorvete, xarope, gelo) chama um destes métodos enquanto
// a xícara estiver na mão. O "tipo" da bebida nunca é escolhido diretamente —
// ele emerge da combinação desses ingredientes, igual na vida real.
public class DrinkBuilder : MonoBehaviour
{
    public static DrinkBuilder Instance { get; private set; }

    public PreparedDrink Current { get; private set; } = new PreparedDrink();
    public bool HasCup { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Chamado ao pegar uma xícara nova: começa uma bebida do zero.
    public void StartNewCup(CupSize size)
    {
        Current = new PreparedDrink { Quantity = 1, Size = size };
        HasCup = true;
    }

    // Puxar um shot na máquina de espresso sempre define a base como Espresso.
    public void AddShot()
    {
        Current.Base = BaseIngredient.Espresso;
        Current.Shots++;
    }

    // Usar uma base alternativa (Chai/Matcha/Earl Grey) — não passa pela máquina de espresso.
    public void SetBase(BaseIngredient baseIngredient) => Current.Base = baseIngredient;

    // Cada uso do dispenser de leite aumenta a quantidade em um nível
    // (Nenhum -> Pitada -> Meio a Meio -> Cheio).
    public void AddMilk(MilkType type)
    {
        Current.Milk = type;
        Current.MilkAmount = NextMilkAmount(Current.MilkAmount);
    }

    private static MilkAmount NextMilkAmount(MilkAmount current)
    {
        switch (current)
        {
            case MilkAmount.None: return MilkAmount.Splash;
            case MilkAmount.Splash: return MilkAmount.Half;
            default: return MilkAmount.Full;
        }
    }

    public void SetFoamed(bool foamed) => Current.Foamed = foamed;
    public void SetWaterAdded(bool added) => Current.WaterAdded = added;
    public void SetIceCreamAdded(bool added) => Current.IceCreamAdded = added;
    public void SetIced(bool iced) => Current.Iced = iced;
    public void SetFlavor(string flavor) => Current.Flavor = flavor;

    // Chamado quando o jogador solta a xícara em espaço vazio: entrega o
    // pedido, devolve a xícara pro lugar de origem e libera as mãos.
    public void Deliver(CupPickup cup)
    {
        GameManager.Instance.SubmitOrder(Current);
        cup.ReturnToOriginalSpot();
        HasCup = false;
        Current = new PreparedDrink();
    }

    // Chamado ao clicar em espaço vazio de mãos livres: avança pro próximo
    // pedido depois de ver o feedback da entrega.
    public void TryAdvanceAfterFeedback()
    {
        if (GameManager.Instance.CurrentState == GameState.ShowingFeedback)
        {
            GameManager.Instance.ProceedToNextOrder();
        }
    }
}
