using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ChiliGames.VRClassroom.PlatformManager;
using static PlayFabLogin;

public class MenuManager : MonoBehaviour
{
    [Space(10)]
    [Header("SignUp")]
    [SerializeField] TMP_InputField email_Input, username_Input, password_Input;
    [SerializeField] HorizontalSelector horizontalSelector_signUp_teacher_student;
    [SerializeField] HorizontalSelector horizontalSelector_signUp_Subject;
    [SerializeField] ButtonManager signUp_Button;
    [SerializeField] Button goToLoginPanelButton;
    [SerializeField] GameObject signUpPanel;

    [Space(10)]
    [Header("Login")]
    [SerializeField] TMP_InputField email_Input_login, password_Input_login;
    [SerializeField] ButtonManager login_Button;
    [SerializeField] Button goToSignUpButton;
    [SerializeField] GameObject loginPanel;

    [Space(10)]
    [Header("Student")]
    public string selectedSubject_student = "English";
    [SerializeField] GameObject teacherPanel;
    [SerializeField] HorizontalSelector horizontalSelector_student_subject;

    [Space(10)]
    [Header("Teacher")]
    public TextMeshProUGUI teacherSubjectText;
    [SerializeField] GameObject studentPanel;

    [SerializeField] public GameObject loadingPanel;

    string selectedSub = "English";
    string selectedRole = "Student";

    public menuState currentMenuState;

    public Mode mode;


    public static MenuManager instance;



    private void Awake()
    {
        instance = this;

        signUp_Button.onClick.AddListener(() => SignUp());

        horizontalSelector_signUp_teacher_student.onValueChanged.AddListener((int value) => OnTeacherStudentChanged(value));

        horizontalSelector_signUp_Subject.gameObject.SetActive(false);

        horizontalSelector_signUp_Subject.onValueChanged.AddListener((int value) => OnSubjectChanged(value));

        login_Button.onClick.AddListener(() => Login());

        goToSignUpButton.onClick.AddListener(() => setState(menuState.SignUp));

        goToLoginPanelButton.onClick.AddListener(() => setState(menuState.Login));

        loadingPanel.SetActive(true);

        horizontalSelector_student_subject.onValueChanged.AddListener((int value) => selectedSubject_student = horizontalSelector_student_subject.items[value].itemTitle);
        
    }

    #region HelperMethods

    private void OnSubjectChanged(int value)
    {
        switch (value)
        {
            case 0:
                selectedSub = "English";
                break;
            case 1:
                selectedSub = "Math";
                break;
            case 2:
                selectedSub = "Science";
                break;
        
        }
    }
    private void OnTeacherStudentChanged(int value)
    {
        if(value == 1)
        {
            horizontalSelector_signUp_Subject.gameObject.SetActive(true);
            selectedRole = "Teacher";
        }
        else
        {
            horizontalSelector_signUp_Subject.gameObject.SetActive(false);
            selectedRole = "Student";
        }
    }

    public void setState(menuState menuState)
    {
        switch(menuState)
        {
            case menuState.SignUp:
                signUpPanel.SetActive(true);
                loginPanel.SetActive(false);
                break;
            case menuState.Login:
                loginPanel.SetActive(true);
                signUpPanel.SetActive(false);
                loadingPanel.SetActive(false);
                break;
                case menuState.loading:
                loadingPanel.SetActive(true);
                break;
                case menuState.teacherPanel:
                teacherPanel.SetActive(true);
                studentPanel.SetActive(false);
                loginPanel.SetActive(false);
                signUpPanel.SetActive(false);
                break;
                case menuState.studentPanel:
                teacherPanel.SetActive(false);
                studentPanel.SetActive(true);
                loginPanel.SetActive(false);
                signUpPanel.SetActive(false);
                break;

        }
    }

    #endregion

    #region SignUpLogin
    private async void SignUp()
    {
        loadingPanel.SetActive(true);
        // Conditionally set the subject value
        string subjectToRegister = selectedRole == "Teacher" ? selectedSub : string.Empty;

        // Call Register function
        bool isRegistered = await PlayFabLogin.Register(
            email_Input.text,
            password_Input.text,
            username_Input.text,
            selectedRole,
            subjectToRegister
        );

        // Handle success or failure
        if (isRegistered)
        {
            Debug.Log("Sign-up successful! You can now log in.");
            // Optionally: Navigate to login UI or show success message
        }
        else
        {
            Debug.LogError("Sign-up failed. Please check your details and try again.");
            // Optionally: Show error message on UI
        }
        loadingPanel.SetActive(false);
    }


    private async void Login()
    {
        loadingPanel.SetActive(true);
        // Call Login function
        bool isLoggedIn = await PlayFabLogin.Login(
            email_Input_login.text,
            password_Input_login.text
        );

        // Handle success or failure
        if (isLoggedIn)
        {
            Debug.Log("Login successful!");
            Debug.Log("Role: " + PlayFabLogin.UserData.Role);
            Debug.Log("Subject: " + PlayFabLogin.UserData.Subject);
            // Optionally: Navigate to the main game scene
            // SceneManager.LoadScene("MainScene");

            if(UserData.Role == "Teacher")
            {
                mode = Mode.Teacher;
                teacherSubjectText.text = "You are Teacher of : " + PlayFabLogin.UserData.Subject;
                setState(menuState.teacherPanel);
                

            }
            else if(UserData.Role == "Student")
            {
                mode = Mode.StudentVR;
                
                setState(menuState.studentPanel);
                
            }
        }
        else
        {
            Debug.LogError("Login failed. Please check your credentials.");
            // Optionally: Show error message on UI
        }
        loadingPanel.SetActive(false);
    }

    #endregion
}
public enum menuState
{
    loading,
    SignUp,
    Login,
    teacherPanel,
    studentPanel

}