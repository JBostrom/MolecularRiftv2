using UnityEngine;
using System.Collections;

public class PopulateLigandMenu : MonoBehaviour {

    public AddAtoms addAtoms { get; set; }

    private GameObject background;
    private float backgroundHeight;
    private float startHeight;
    private int itemsPerPage;
    private int currentPage;

    public GameObject menuItem;
    private GameObject ligandMenuSlider;
    private LigandMenuSliderDataBinder ligandMenuSliderDataBinder;
    private GameObject[] menuItems;
    

	void Start ()
    {
        addAtoms = transform.parent.parent.GetComponent<AddAtoms>();

        itemsPerPage = 5;

        ligandMenuSlider = transform.Find("LigandMenuSlider").gameObject;
        ligandMenuSliderDataBinder = ligandMenuSlider.GetComponent<LigandMenuSliderDataBinder>();

        background = transform.Find("BackgroundCanvas").gameObject;
        backgroundHeight = background.GetComponent<RectTransform>().rect.height;
        startHeight = backgroundHeight / 2 - menuItem.transform.Find("TextCanvas").gameObject.GetComponent<RectTransform>().rect.height / 2;
        
        if (addAtoms.CollectionOfLigands != null)
        {
            if (addAtoms.CollectionOfLigands.Count < itemsPerPage)
            {
                menuItems = new GameObject[addAtoms.CollectionOfLigands.Count];
            }
            else
            {
                menuItems = new GameObject[itemsPerPage];
            }

            GameObject temp;
            float menuItemHeight = menuItem.transform.Find("TextCanvas").GetComponent<RectTransform>().rect.height;
            for (int i = 0; i < addAtoms.CollectionOfLigands.Count; i++)
            {
                if (i < itemsPerPage)
                {
                    temp = Instantiate(menuItem) as GameObject;
                    temp.transform.parent = transform;
                    temp.transform.localScale = Vector3.one;
                    temp.transform.localPosition = Vector3.up * (startHeight - menuItemHeight * i);
                    temp.transform.Find("LigandToggleButton").GetComponent<ToggleLigandButtonDataBinder>().LigandIndex = i;
                    temp.transform.Find("TextCanvas/LigandText").GetComponent<UnityEngine.UI.Text>().text = "Ligand" + (i + 1).ToString();
                    menuItems[i] = temp;
                }
            }
        }
        
        if (addAtoms.CollectionOfLigands.Count > itemsPerPage)
        {
            var pages = (addAtoms.CollectionOfLigands.Count / itemsPerPage) - 0.1f;
            if (addAtoms.CollectionOfLigands.Count % itemsPerPage != 0)
            {
                pages += 1;
            }
            
            ligandMenuSliderDataBinder.max = pages;
            ligandMenuSliderDataBinder.outputValue = 0;
        }
        else
        {
            ligandMenuSlider.SetActive(false);
        }

	}
	
    public void Pagination(float sliderValue)
    {
        int page = (int)Mathf.Floor(sliderValue);
        if (page != currentPage)
        {
            currentPage = page;
            ToggleLigandButtonDataBinder buttonDataBinder;
            for (int i = 0; i < itemsPerPage; i++)
            {
                if (page * itemsPerPage + i < addAtoms.CollectionOfLigands.Count)
                {
                    menuItems[i].SetActive(true);
                    buttonDataBinder = menuItems[i].transform.Find("LigandToggleButton").GetComponent<ToggleLigandButtonDataBinder>();
                    buttonDataBinder.LigandIndex = page * itemsPerPage + i;
                    buttonDataBinder.GetCurrentData();
                    if (addAtoms.LigandsShowing[buttonDataBinder.LigandIndex])
                    {
                        buttonDataBinder.gameObject.transform.Find("Button").GetComponent<ButtonDemoToggle>().ButtonTurnsOn();
                    }
                    else
                    {
                        buttonDataBinder.gameObject.transform.Find("Button").GetComponent<ButtonDemoToggle>().ButtonTurnsOff();
                    }

                    menuItems[i].transform.Find("TextCanvas/LigandText").GetComponent<UnityEngine.UI.Text>().text = "Ligand" + (page * itemsPerPage + i + 1).ToString();
                }
                else
                {
                    menuItems[i].SetActive(false);
                }
            }
        }
    }

	void Update ()
    {
	    if (addAtoms == null)
        {
            addAtoms = transform.parent.parent.GetComponent<AddAtoms>();
        }
    }
}
