using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class commonSenseScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMNeedyModule needyModule;

    public TextMesh[] texts; //0 = screen, 1 = left, 2 = right
    public KMSelectable[] buttons; //0 = screen, 1 = left, 2 = right
    public Material[] mats; //0 = normal, 1 = red
    public GameObject theBack;
    KMAudio.KMAudioRef soundEffect;

    int puzzleType = 0;
    int rng = 0;
    int edge = 0; //0= truth, 1= lie
    int answer = 0; //0= left, 1= right
    string rngString = "";

    private List<int> fireAnswer = new List<int> { 0, 1, 1, 0, 1, 0, 0, 1 };
    public Color[] colors;
    private List<String> colorNames = new List<string> { "Red", "Orange", "Yellow", "Green", "Blue", "Purple" };
    public AudioClip[] animalSounds;
    private List<String> animalNames = new List<string> { "Bee", "Cat", "Cow", "Dog", "Duck", "Frog", "Horse", "Lion", "Owl", "Pig", "Rooster", "Sheep" };
    int animal = 0;
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake() {
        moduleId = moduleIdCounter++;
        needyModule = GetComponent<KMNeedyModule>();
        needyModule.OnNeedyActivation += OnNeedyActivation;
        needyModule.OnNeedyDeactivation += OnNeedyDeactivation;
        needyModule.OnTimerExpired += OnTimerExpired;

        foreach (KMSelectable button in buttons) {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { buttonPress(pressedButton); return false; };
        }
    }

    // Use this for initialization
    void Start() {
        moduleSolved = true;
    }

    void Update() {
        if (moduleSolved == true)
        {
            texts[0].text = "Common Sense";
            texts[1].text = "-";
            texts[2].text = "-";
            theBack.GetComponent<MeshRenderer>().material = mats[0];
            texts[0].color = colors[6];
        }
    }

    void OnNeedyActivation()
    {
        moduleSolved = false;
        edge = UnityEngine.Random.Range(0, 2);
        puzzleType = UnityEngine.Random.Range(0, 10);
        if (puzzleType == 0) //Press the (left/right) button.
        {
            rng = UnityEngine.Random.Range(0, 2);
            if (rng == 0)
            {
                rngString = "left";
                answer = 0;
            } else
            {
                rngString = "right";
                answer = 1;
            }
            texts[0].text = "Press the \n" + rngString + " button.";
            texts[1].text = "<";
            texts[2].text = ">";
        } else if (puzzleType == 1) //Would you like to blow up right now?
        {
            rng = UnityEngine.Random.Range(0, 2);
            if (rng == 0)
            {
                texts[1].text = "No.";
                texts[2].text = "Yes.";
                answer = 0;
            } else
            {
                texts[1].text = "Yes.";
                texts[2].text = "No.";
                answer = 1;
            }
            texts[0].text = "Would you like to\nblow up right now?";
        } else if (puzzleType == 2) //Is (fire/ice) (hot/cold)?
        {
            rng = UnityEngine.Random.Range(0, 8);
            if (rng < 4)
            {
                rngString = "fire";
            } else
            {
                rngString = "ice";
            }

            if ((rng % 4) < 2)
            {
                texts[0].text = "Is " + rngString + " hot?";
            } else
            {
                texts[0].text = "Is " + rngString + " cold?";
            }

            if ((rng % 2) == 0)
            {
                texts[1].text = "Yes.";
                texts[2].text = "No.";
            } else
            {
                texts[1].text = "No.";
                texts[2].text = "Yes.";
            }

            answer = fireAnswer[rng];
        } else if (puzzleType == 3) //What is the (first/last) letter of ‘<small word>’?
        {
            rng = UnityEngine.Random.Range(0, 1004);
            rngString = wordList.words[rng];

            rng = UnityEngine.Random.Range(0, 4);
            if (rng == 0)
            {
                texts[0].text = "What is the first\nletter in the word\n\'" + rngString + "\'?";
                texts[1].text = " " + rngString[0] + " ";
                if (rngString[0] != rngString[rngString.Length - 1])
                {
                    rngString = " " + rngString[rngString.Length - 1] + " ";
                } else
                {
                    ROT13(rngString[0]);
                }
                texts[2].text = rngString;
                answer = 0;
            } else if (rng == 1)
            {
                texts[0].text = "What is the first\nletter in the word\n\'" + rngString + "\'?";
                texts[2].text = " " + rngString[0] + " ";
                if (rngString[0] != rngString[rngString.Length - 1])
                {
                    rngString = " " + rngString[rngString.Length - 1] + " ";
                }
                else
                {
                    ROT13(rngString[0]);
                }
                texts[1].text = rngString;
                answer = 1;
            } else if (rng == 2)
            {
                texts[0].text = "What is the last\nletter in the word\n\'" + rngString + "\'?";
                texts[1].text = " " + rngString[rngString.Length - 1] + " ";
                if (rngString[0] != rngString[rngString.Length - 1])
                {
                    rngString = " " + rngString[0] + " ";
                }
                else
                {
                    ROT13(rngString[rngString.Length - 1]);
                }
                texts[2].text = rngString;
                answer = 0;
            } else
            {
                texts[0].text = "What is the last\nletter in the word\n\'" + rngString + "\'?";
                texts[2].text = " " + rngString[rngString.Length - 1] + " ";
                if (rngString[0] != rngString[rngString.Length - 1])
                {
                    rngString = " " + rngString[0] + " ";
                }
                else
                {
                    ROT13(rngString[rngString.Length - 1]);
                }
                texts[1].text = rngString;
                answer = 1;
            }
        } else if (puzzleType == 4) //What color is this text? [Text is colored]
        {
            rng = UnityEngine.Random.Range(0, 6);
            texts[0].text = "What's the color\nof this text?";
            texts[0].color = colors[rng];
            rngString = colorNames[rng];
            Debug.LogFormat("[Common Sense #{0}] The color of the text is: {1}", moduleId, rngString);
            rng = UnityEngine.Random.Range(0, 2);
            if (rng == 0)
            {
                texts[1].text = rngString;
                ColorOpp(rngString);
                texts[2].text = rngString;
                answer = 0;
            } else
            {
                texts[2].text = rngString;
                ColorOpp(rngString);
                texts[1].text = rngString;
                answer = 1;
            }
        } else if (puzzleType == 5) //Press the screen, not the buttons.
        {
            texts[0].text = "Press the screen,\nnot the buttons.";
            texts[1].text = "-";
            texts[2].text = "-";
            answer = 2;
        } else if (puzzleType == 6) //Let the needy timer run out, don’t press anything.
        {
            texts[0].text = "Let the needy timer\n run out, don't\npress anything.";
            texts[1].text = "-";
            texts[2].text = "-";
            answer = 4;
        } else if (puzzleType == 7) //What animal sound do you get when you press the screen?
        {
            rng = UnityEngine.Random.Range(0, 12);
            animal = rng;
            rngString = animalNames[animal];
            Debug.LogFormat("[Common Sense #{0}] The animal sound that plays is: {1}", moduleId, rngString);
            texts[0].text = "What animal sound\ndo you get when you\npress the screen?";
            rng = UnityEngine.Random.Range(0, 2);
            if (rng == 0)
            {
                rng = UnityEngine.Random.Range(1, 11);
                texts[1].text = rngString;
                texts[2].text = animalNames[(animal + rng) % 12];
                answer = 0;
            } else
            {
                rng = UnityEngine.Random.Range(1, 11);
                texts[2].text = rngString;
                texts[1].text = animalNames[(animal + rng) % 12];
                answer = 1;
            }
        } else if (puzzleType == 8) //What number comes (before/after) <single-digit-number>?
        {
            rng = UnityEngine.Random.Range(0, 4);
            if (rng == 0)
            {
                rng = UnityEngine.Random.Range(1, 8);
                texts[0].text = "What number comes\nbefore "+ rng +"?";
                texts[1].text = " " + (rng - 1) + " ";
                texts[2].text = " " + (rng + 1) + " ";
                answer = 0;
            }
            else if (rng == 1)
            {
                rng = UnityEngine.Random.Range(1, 8);
                texts[0].text = "What number comes\nbefore " + rng + "?";
                texts[1].text = " " + (rng + 1) + " ";
                texts[2].text = " " + (rng - 1) + " ";
                answer = 1;
            }
            else if (rng == 2)
            {
                rng = UnityEngine.Random.Range(1, 8);
                texts[0].text = "What number comes\nafter " + rng + "?";
                texts[1].text = " " + (rng + 1) + " ";
                texts[2].text = " " + (rng - 1) + " ";
                answer = 0;
            } else
            {
                rng = UnityEngine.Random.Range(1, 8);
                texts[0].text = "What number comes\nafter " + rng + "?";
                texts[1].text = " " + (rng - 1) + " ";
                texts[2].text = " " + (rng + 1) + " ";
                answer = 1;
            }
        }
        else if (puzzleType == 9) //What letter comes (before/after) <letter>?
        {
            rng = UnityEngine.Random.Range(0, 4);
            if (rng == 0)
            {
                rng = UnityEngine.Random.Range(1, 24);
                texts[0].text = "What letter comes\nbefore " + alphabet[rng] + "?";
                texts[1].text = " " + alphabet[(rng - 1)] + " ";
                texts[2].text = " " + alphabet[(rng + 1)] + " ";
                answer = 0;
            }
            else if (rng == 1)
            {
                rng = UnityEngine.Random.Range(1, 24);
                texts[0].text = "What letter comes\nbefore " + alphabet[rng] + "?";
                texts[1].text = " " + alphabet[(rng + 1)] + " ";
                texts[2].text = " " + alphabet[(rng - 1)] + " ";
                answer = 1;
            }
            else if (rng == 2)
            {
                rng = UnityEngine.Random.Range(1, 24);
                texts[0].text = "What letter comes\nafter " + alphabet[rng] + "?";
                texts[1].text = " " + alphabet[(rng + 1)] + " ";
                texts[2].text = " " + alphabet[(rng - 1)] + " ";
                answer = 0;
            }
            else
            {
                rng = UnityEngine.Random.Range(1, 24);
                texts[0].text = "What letter comes\nafter " + alphabet[rng] + "?";
                texts[1].text = " " + alphabet[(rng - 1)] + " ";
                texts[2].text = " " + alphabet[(rng + 1)] + " ";
                answer = 1;
            }
        }

        Debug.LogFormat("[Common Sense #{0}] The display says: {1}", moduleId, texts[0].text);
        Debug.LogFormat("[Common Sense #{0}] The left button says: {1}", moduleId, texts[1].text);
        Debug.LogFormat("[Common Sense #{0}] The right button says: {1}", moduleId, texts[2].text);
        if (edge == 1)
        {
            answer = (answer + 1) % 2;
            theBack.GetComponent<MeshRenderer>().material = mats[1];
            Debug.LogFormat("[Common Sense #{0}] The edge is colored red.", moduleId);
            if (puzzleType == 5)
            {
                answer = 3;
            } else if (puzzleType == 6)
            {
                answer = 5;
            }
        } else
        {
            theBack.GetComponent<MeshRenderer>().material = mats[0];
            Debug.LogFormat("[Common Sense #{0}] The edge is colored green.", moduleId);
        }

        if (answer == 0)
        {
            Debug.LogFormat("[Common Sense #{0}] The correct answer is the left button ({1}).", moduleId, texts[1].text);
        } else if (answer == 1)
        {
            Debug.LogFormat("[Common Sense #{0}] The correct answer is the right button ({1}).", moduleId, texts[2].text);
        } else if (answer == 2)
        {
            Debug.LogFormat("[Common Sense #{0}] The correct answer is the screen.", moduleId);
        } else if (answer == 3)
        {
            Debug.LogFormat("[Common Sense #{0}] The correct answer is either of the buttons.", moduleId);
        } else if (answer == 4)
        {
            Debug.LogFormat("[Common Sense #{0}] The correct answer is to let the countdown timer run out.", moduleId);
        } else if (answer == 5)
        {
            Debug.LogFormat("[Common Sense #{0}] The correct answer is either of the buttons or the screen.", moduleId);
        }
    }

    void OnNeedyDeactivation()
    {
        texts[0].text = "Common Sense";
        texts[1].text = "-";
        texts[2].text = "-";
        theBack.GetComponent<MeshRenderer>().material = mats[0];
        texts[0].color = colors[6];
        moduleSolved = true;
    }

    void OnTimerExpired()
    {
        if (answer != 4)
        {
            needyModule.HandleStrike();
            texts[0].text = "Common Sense";
            texts[1].text = "-";
            texts[2].text = "-";
            theBack.GetComponent<MeshRenderer>().material = mats[0];
            texts[0].color = colors[6];
            Debug.LogFormat("[Common Sense #{0}] You didn't input your response in time.", moduleId);
            moduleSolved = true;
        } else
        {
            needyModule.HandlePass();
            Debug.LogFormat("[Common Sense #{0}] You selected the correct answer. Pass.", moduleId);
        }
    }

    void buttonPress(KMSelectable button)
    {
        button.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (moduleSolved == false)
        {
            if (button == buttons[0])
            {
                if (puzzleType == 5)
                {
                    if (answer == 2)
                    {
                        needyModule.HandlePass();
                        Debug.LogFormat("[Common Sense #{0}] You selected the correct answer. Pass.", moduleId);
                        moduleSolved = true;
                    }
                    else
                    {
                        needyModule.HandleStrike();
                        needyModule.HandlePass();
                        Debug.LogFormat("[Common Sense #{0}] You selected the wrong answer. Strike!", moduleId);
                        moduleSolved = true;
                    }
                } else if (puzzleType == 6)
                {
                    if (answer == 5)
                    {
                        needyModule.HandlePass();
                        Debug.LogFormat("[Common Sense #{0}] You selected the correct answer. Pass.", moduleId);
                        moduleSolved = true;
                    }
                    else
                    {
                        needyModule.HandleStrike();
                        needyModule.HandlePass();
                        Debug.LogFormat("[Common Sense #{0}] You selected the wrong answer. Strike!", moduleId);
                        moduleSolved = true;
                    }
                } else if (puzzleType == 7)
                {
                    Audio.PlaySoundAtTransform(animalNames[animal], needyModule.transform);
                }
            }
            else if (button == buttons[1])
            {
                if (answer == 0 || answer == 3 || answer == 5)
                {
                    needyModule.HandlePass();
                    Debug.LogFormat("[Common Sense #{0}] You selected the correct answer. Pass.", moduleId);
                    moduleSolved = true;
                }
                else
                {
                    needyModule.HandleStrike();
                    needyModule.HandlePass();
                    Debug.LogFormat("[Common Sense #{0}] You selected the wrong answer. Strike!", moduleId);
                    moduleSolved = true;
                }
            }
            else if (button == buttons[2])
            {
                if (answer == 1 || answer == 3 || answer == 5)
                {
                    needyModule.HandlePass();
                    Debug.LogFormat("[Common Sense #{0}] You selected the correct answer. Pass.", moduleId);
                    moduleSolved = true;
                }
                else
                {
                    needyModule.HandleStrike();
                    needyModule.HandlePass();
                    Debug.LogFormat("[Common Sense #{0}] You selected the wrong answer. Strike!", moduleId);
                    moduleSolved = true;
                }
            }
            else
            {
                Debug.Log("Houston, we fucked up!");
            }
        }
    }

    void ROT13(char c)
    {
        switch (c)
        {
            case 'a':
                rngString = " n ";
                break;
            case 'b':
                rngString = " o ";
                break;
            case 'c':
                rngString = " p ";
                break;
            case 'd':
                rngString = " q ";
                break;
            case 'e':
                rngString = " r ";
                break;
            case 'f':
                rngString = " s ";
                break;
            case 'g':
                rngString = " t ";
                break;
            case 'h':
                rngString = " u ";
                break;
            case 'i':
                rngString = " v ";
                break;
            case 'j':
                rngString = " w ";
                break;
            case 'k':
                rngString = " x ";
                break;
            case 'l':
                rngString = " y ";
                break;
            case 'm':
                rngString = " z ";
                break;
            case 'n':
                rngString = " a ";
                break;
            case 'o':
                rngString = " b ";
                break;
            case 'p':
                rngString = " c ";
                break;
            case 'q':
                rngString = " d ";
                break;
            case 'r':
                rngString = " e ";
                break;
            case 's':
                rngString = " f ";
                break;
            case 't':
                rngString = " g ";
                break;
            case 'u':
                rngString = " h ";
                break;
            case 'v':
                rngString = " i ";
                break;
            case 'w':
                rngString = " j ";
                break;
            case 'x':
                rngString = " k ";
                break;
            case 'y':
                rngString = " l ";
                break;
            case 'z':
                rngString = " m ";
                break;
            default:
                rngString = " RIP ";
                break;
        }
        return;
    }

    void ColorOpp(string s)
    {
        switch (s)
        {
            case "Red":
                rngString = "Green";
                break;
            case "Orange":
                rngString = "Blue";
                break;
            case "Yellow":
                rngString = "Purple";
                break;
            case "Green":
                rngString = "Red";
                break;
            case "Blue":
                rngString = "Orange";
                break;
            case "Purple":
                rngString = "Yellow";
                break;
            default:
                rngString = "RIP";
                break;
        }
    }
}
