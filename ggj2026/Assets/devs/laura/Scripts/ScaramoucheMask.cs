using UnityEngine;

public class ScaramoucheMask : BaseMask
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject bombprefab;
    [SerializeField] private GameObject BombShootPoint;

    [SerializeField] private GameObject knifeprefab;
    [SerializeField] private GameObject KnifeShootPoint;
    
    
    void Start()
    {
        BombShootPoint = transform.Find("ShootPoint").gameObject;
        KnifeShootPoint = transform.Find("ShootPoint").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowBomb();
        }
        

    }

    public override void MainAtackMask()
    {
        
    }

    private void ThrowBomb()
    {
        Debug.Log("throws bombs");
        Instantiate(bombprefab, BombShootPoint.transform.position, BombShootPoint.transform.rotation);
        
    }

    private void ThrowKnife()
    {
        Debug.Log("throws knifes");
        Instantiate(knifeprefab, KnifeShootPoint.transform.position, KnifeShootPoint.transform.rotation);
    }
}
