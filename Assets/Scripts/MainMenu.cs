using UnityEngine;
                using UnityEngine.SceneManagement;
                using UnityEngine.UI;
                
                public class MainMenu : MonoBehaviour
                {
                    private bool setPlay = false;
                    private bool setExit = false;
                    private float fillTimer = 0f;
                
                    [Header("Node IDs")]
                    public NodeID playNodeID;
                    public NodeID exitNodeID;
                    public NodeID centerNodeID;
                
                    [Header("UI Controle")]
                    public string startLevelSceneName;
                    public GameObject Player;
                    public Slider PlaySlider;
                    public Slider ExitSlider;
                    public float fillSpeed = 0.5f;
                
                    private void Update()
                    {
                        if (setPlay)
                        {
                            fillTimer += fillSpeed * Time.deltaTime;
                            float t = Mathf.Clamp01(fillTimer);
                            float fill = Mathf.Log10(1 + t * 9);
                            PlaySlider.value = fill;
                            if (fill >= 1f)
                            {
                                SceneManager.LoadScene(startLevelSceneName);
                                fillTimer = 0f;
                                setPlay = false;
                            }
                        }
                        else
                        {
                            PlaySlider.value = 0;
                        }
                
                        if (setExit)
                        {
                            fillTimer += fillSpeed * Time.deltaTime;
                            float t = Mathf.Clamp01(fillTimer);
                            float fill = Mathf.Log10(1 + t * 9);
                            ExitSlider.value = fill;
                            if (fill >= 1f)
                            {
                                Application.Quit();
                                fillTimer = 0f;
                                setExit = false;
                            }
                        }
                        else
                        {
                            ExitSlider.value = 0;
                        }
                
                        if (!setPlay && !setExit)
                        {
                            fillTimer = 0f;
                        }
                    }
                
                    public void PlayGame()
                    {
                        if (!setPlay)
                        {
                            setPlay = true;
                            setExit = false;
                            Player.GetComponent<Player>().HandleNodeSelected3(playNodeID);
                            fillTimer = 0f;
                        }
                        else
                        {
                            setPlay = false;
                            Player.GetComponent<Player>().HandleNodeSelected3(centerNodeID);
                            fillTimer = 0f;
                        }
                    }
                
                    public void QuitGame()
                    {
                        if (!setExit)
                        {
                            setExit = true;
                            setPlay = false;
                            Player.GetComponent<Player>().HandleNodeSelected3(exitNodeID);
                            fillTimer = 0f;
                        }
                        else
                        {
                            setExit = false;
                            Player.GetComponent<Player>().HandleNodeSelected3(centerNodeID);
                            fillTimer = 0f;
                        }
                    }
                }