using MonoMod;

namespace Modding.Patches;

[MonoModPatch("global::PlayerData")]
public class PlayerData : global::PlayerData
{
    [MonoModReplace]
    public new void TakeHealth(int amount)
    {
        amount = ModHooks.OnTakeHealth(amount);
        if (amount <= 0)
            return;
        
        if (this.healthBlue > 0)
        {
            this.damagedBlue = true;
            this.healthBlue -= amount;
            if (this.healthBlue < 0)
                this.health += this.healthBlue;
        }
        else
        {
            this.damagedBlue = false;
            if (this.health - amount <= 0)
                this.health = 0;
            else
                this.health -= amount;
        }
    }
}