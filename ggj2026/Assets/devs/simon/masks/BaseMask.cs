using UnityEngine;

public class BaseMask : MonoBehaviour
{
    Player player;
    //main atack mask
    virtual public void MainAtackMask()
    {
        Debug.Log("Base Mask Main Atack");
    }



    //secondary atack mask
    virtual public void SecondaryAtackMask()
    {
        Debug.Log("Base Mask Secondary Atack");
    }


    //set mask stats
    virtual public void SetMaskStats()
    {
        Debug.Log("Base Mask Set Stats");
    }

}
