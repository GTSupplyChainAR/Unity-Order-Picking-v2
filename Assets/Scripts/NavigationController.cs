using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System;
using System.Threading;
//using System.Collections.Generic;
//using System.Globalization;

public class NavigationController : MonoBehaviour {


    // data model;
    private int selectedUserId = 0;
    private int selectedPhase = 0; // 0 indicates training, 1 indicates testing
    private int selectedPathId = 1;
    private int selectedBookNum = 0;
    private string selectedBookTag = "";
    private bool [] bookPicked;
    private int numOfPickedBook;
    private PathReader pr;
    private const string url = "https://eyegaze4605api.herokuapp.com/api/userData";
    /* view style config
    private Color selected_color = Color.blue;
    private Color unselected_color = Color.white;*/
    private int sleepTime = 250;
    // views
    GameObject userSelectionView;
    GameObject phaseSelectionView;
    GameObject pathIdSelectionView;
    GameObject bookInfoView;
    GameObject shelfView;
    GameObject completionView;

    // active view
    GameObject currentActiveView;

    // Use this for initialization
    void Start () {
        Console.WriteLine("NavigationController Starting");
        // data model init
        pr = new PathReader(Path.Combine(Application.streamingAssetsPath, "pick-paths.json"));
        pr.setPathId(selectedPathId);
        numOfPickedBook = 0;
        userSelectionView = GameObject.Find("User Selection View");
        userSelectionView.SetActive(true);
        phaseSelectionView = GameObject.Find("Phase Selection View");
        phaseSelectionView.SetActive(false);
        pathIdSelectionView = GameObject.Find("PathId Selection View");
        pathIdSelectionView.SetActive(false);
        bookInfoView = GameObject.Find("Book Info View");
        bookInfoView.SetActive(false);
        shelfView = GameObject.Find("Shelf View");
        shelfView.GetComponent<ShelfView>().init();
        shelfView.SetActive(false);
        
        completionView = GameObject.Find("Completion View");
        completionView.SetActive(false);
        currentActiveView = userSelectionView;


    }
    private void postdata() {
        Console.WriteLine("NavigationController PostingData");

        WWWForm form = new WWWForm();
        form.AddField("userId", selectedUserId);
        form.AddField("phase", selectedPhase);
        form.AddField("time", (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        form.AddField("pathId", selectedPathId);
        form.AddField("bookTag", selectedBookTag);
        form.AddField("device", 2);
        form.AddField("viewPosition", 1);
        StartCoroutine(Upload(form));
    }
    private IEnumerator Upload(WWWForm form) {
        var download = UnityWebRequest.Post(url, form);
        yield return download.SendWebRequest();
        if (download.isNetworkError || download.isHttpError)
        {

        }
        else {

        }
    }

    /*if ((Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick2Button0)) && bookNum < pr.getNumberOfBooksInPath() - 1)
    {
    }*/
    private IEnumerator holdOn()
    {
        print("S " + Time.time);
        yield return new WaitForSeconds(1);
        print("E " + Time.time);
    }
    int x = 0;
    private void userSelectionControl() {
        //Debug.Log("V " + Input.GetAxis("Vertical"));
        //Debug.Log("H " + Input.GetAxis("Horizontal"));
        /*
                2   
            0       3
                1
         */
        //if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Joystick2Button1))
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick2Button1))
        {
            
               userSelectionView.GetComponent<UserSelectionView>().selectNext();
            
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Joystick2Button2))
        {
            
                userSelectionView.GetComponent<UserSelectionView>().selectLast();
            
        }
        else if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Joystick2Button3))
        {
           
                selectedUserId = userSelectionView.GetComponent<UserSelectionView>().getSelectedUserId();
                currentActiveView.SetActive(false);
                phaseSelectionView.SetActive(true);
                // clear next selection
                selectedPhase = 0;
                phaseSelectionView.GetComponent<PhaseSelectionView>().setPhase(selectedPhase);
                currentActiveView = phaseSelectionView;
            
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick2Button0)) {
            // no action
        }
    }

    private void phaseSelectionControl()
    {
        //if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Joystick2Button1))
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick2Button1))
        {
            phaseSelectionView.GetComponent<PhaseSelectionView>().selectTesting();
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Joystick2Button2))
        {
            phaseSelectionView.GetComponent<PhaseSelectionView>().selectTraining();
        }
        else if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Joystick2Button3))
        {
            selectedPhase = phaseSelectionView.GetComponent<PhaseSelectionView>().getSelectedPhase();
            currentActiveView.SetActive(false);
            pathIdSelectionView.SetActive(true);
            currentActiveView = pathIdSelectionView;
            // setup next selection
            pathIdSelectionView.GetComponent<PathIdSelectionView>().setPhase(selectedPhase);
        }
        else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick2Button0))
        {
            // go back to user selection
            currentActiveView.SetActive(false);
            userSelectionView.SetActive(true);
            currentActiveView = userSelectionView;
        }
    }

    private void pathIdSelectionControl() {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick2Button1))
        {
            pathIdSelectionView.GetComponent<PathIdSelectionView>().selectNext();
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Joystick2Button2))
        {
            pathIdSelectionView.GetComponent<PathIdSelectionView>().selectLast();
        }
        else if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Joystick2Button3)) {
            selectedPathId = pathIdSelectionView.GetComponent<PathIdSelectionView>().getSelectedPathId();
            
            currentActiveView.SetActive(false);
            bookInfoView.SetActive(true);
            currentActiveView = bookInfoView;
            // setup the next view
            /*if (selectedPathId != pr.getPathId())
            {*/
            bookPicked = new bool[pr.getNumberOfBooksInPath()];
            for(int i = 0; i < bookPicked.Length; i++)
            {
                bookPicked[i] = false;
            }
            pr.setPathId(selectedPathId);
            numOfPickedBook = 0;
            selectedBookNum = 0;
                
            //}
            bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
        }
        else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick2Button0)) {
            currentActiveView.SetActive(false);
            phaseSelectionView.SetActive(true);
            currentActiveView = phaseSelectionView;

        }

    }
    private void bookInfoControl() {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick2Button1))
        {
            while (selectedBookNum + 1 < bookPicked.Length)
            {
                selectedBookNum++;
                if (!bookPicked[selectedBookNum])
                {
                    bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
                    break;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Joystick2Button2))
        {
            while (selectedBookNum > 0)
            {
                selectedBookNum--;
                if (!bookPicked[selectedBookNum])
                {
                    bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
                    break;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Joystick2Button14)) {
            // switch to shelf view
            currentActiveView.SetActive(false);
            shelfView.SetActive(true);
            currentActiveView = shelfView;
            shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
        }
        else if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Joystick2Button15))
        {
            // get the book, send server data
            if (!bookPicked[selectedBookNum])
            {
                selectedBookTag = pr.getBookWithLocation(selectedBookNum).book.tag;
                //Debug.Log(selectedBookTag + " " + (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
                postdata();
                bookPicked[selectedBookNum] = true;
                numOfPickedBook++;
            }
            while (selectedBookNum + 1 < bookPicked.Length)
            {
                selectedBookNum++;
                if (!bookPicked[selectedBookNum])
                {
                    bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
                    break;
                }
            }
            if (numOfPickedBook >= bookPicked.Length)
            {
                // go to next, or notify completion.
                currentActiveView.SetActive(false);
                completionView.SetActive(true);
                currentActiveView = completionView;
            }
            else {

            }
        }
        else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick2Button0))
        {
            currentActiveView.SetActive(false);
            pathIdSelectionView.SetActive(true);
            currentActiveView = pathIdSelectionView;
        }
    }
    private void completionControl() {
        if (Input.anyKeyDown)
        {
            selectedUserId = 0;
            selectedPhase = 0; // 0 indicates training, 1 indicates testing
            selectedPathId = 1;
            selectedBookNum = 0;
            selectedBookTag = "";
            bookPicked = null;
            numOfPickedBook = 0;
            currentActiveView.SetActive(false);
            userSelectionView.SetActive(true);
            currentActiveView = userSelectionView;
        }
    }
    private void shelfControl() {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick2Button1))
        {
            while (selectedBookNum + 1 < bookPicked.Length)
            {
                selectedBookNum++;
                if (!bookPicked[selectedBookNum])
                {
                    shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
                    break;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Joystick2Button2))
        {
            while (selectedBookNum > 0)
            {
                selectedBookNum--;
                if (!bookPicked[selectedBookNum])
                {

                    shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
                    break;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Joystick2Button14))
        {
            // switch to book info view
            currentActiveView.SetActive(false);
            bookInfoView.SetActive(true);
            currentActiveView = bookInfoView;
            bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
        }
        else if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Joystick2Button15))
        {
            // get the book, send server data
            if (!bookPicked[selectedBookNum])
            {
                selectedBookTag = pr.getBookWithLocation(selectedBookNum).book.tag;
                //Debug.Log(selectedBookTag + " " + (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
                postdata();
                bookPicked[selectedBookNum] = true;
                numOfPickedBook++;
            }
            while (selectedBookNum + 1 < bookPicked.Length)
            {
                selectedBookNum++;
                if (!bookPicked[selectedBookNum])
                {
                    shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
                    break;
                }
            }
            if (numOfPickedBook >= bookPicked.Length)
            {
                // go to next, or notify completion.
                currentActiveView.SetActive(false);
                completionView.SetActive(true);
                currentActiveView = completionView;
            }
            else
            {

            }
        }
        else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick2Button0))
        {
            currentActiveView.SetActive(false);
            pathIdSelectionView.SetActive(true);
            currentActiveView = pathIdSelectionView;
        }
    }
    // Update is called once per frame
    void Update () {
        if (currentActiveView == userSelectionView)
        {
            userSelectionControl();
        }
        else if (currentActiveView == phaseSelectionView)
        {
            phaseSelectionControl();
        }
        else if (currentActiveView == pathIdSelectionView)
        {
            pathIdSelectionControl();
        }
        else if (currentActiveView == bookInfoView)
        {
            bookInfoControl();
        }
        else if (currentActiveView == shelfView)
        {
            shelfControl();
        }
        else if (currentActiveView == completionView)
        {
            completionControl();
        }
    }
}
