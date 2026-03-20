using UnityEngine;
using GameEnum;
using System.Collections.Generic;
public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerStart;
    [SerializeField] private GameObject seat1;
    [SerializeField] private GameObject seat2;
    [SerializeField] private GameObject seat3;
    [SerializeField] private GameObject seat4;
    private GameObject[] seats;
    public static CustomerManager Instance;
    private GameObject[] Customers;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        this.seats = new GameObject[4];
        this.seats[0] = seat1;
        this.seats[1] = seat2;
        this.seats[2] = seat3;
        this.seats[3] = seat4;

        this.Customers = GameObject.FindGameObjectsWithTag("Customer");
        int lenght = this.Customers.Length;
        Debug.Log($"Running CustomerManager with {lenght} Customers");

        float MIN_ORDER_COOLDOWN = 20; // 60
        float MAX_ORDER_COOLDOWN = 40; // 120
        for (int i = 0; i < lenght; i++)
        {
            int curIdx = i;
            GameObject customerObj = this.Customers[curIdx];
            //Renderer renderer = customerObj.GetComponent<MeshRenderer>();
            GameObject bone = customerObj.transform.Find("Bone").gameObject;
            Customer customer = customerObj.GetComponent<Customer>();
            customer.OnOrderReady += () =>
            {
                Debug.Log($"On Order Ready Invoked {curIdx}");
                //renderer.enabled = true;
                bone.SetActive(true);
                customer.StartOrdering();
                customer.orderCooldown = Random.Range(MIN_ORDER_COOLDOWN, MAX_ORDER_COOLDOWN);
                customer.MoveToPoint(this.seats[curIdx].transform.position, new Vector3(-90, 180),null);
                //renderer.enabled = true;
            };
            customer.OnLeaving += (orderState) =>
            {
                Debug.Log($"Customer {curIdx}: Leaving with order state: {orderState}");
                customer.MoveToPoint(customerStart.transform.position, new Vector3(-90, 0), () => {
                    //renderer.enabled = false;
                    bone.SetActive(false);
                });
                if (orderState == OrderState.Success)
                {
                    EventBroadcaster.Instance.PostEvent(ActionEvent.CustomerServed.ToString());
                }
                
            };
            customer.orderCooldown = 10 + curIdx * 30;
            customer.transform.position = customerStart.transform.position;
            customer.StartInCooldown();
        }
    }
    public static void SetResumeCustomerOrdering(bool state)
    {
        foreach (GameObject customer in Instance.Customers)
        {
            customer.GetComponent<Customer>().resumeOrderCooldown = state;
        }
    }
}