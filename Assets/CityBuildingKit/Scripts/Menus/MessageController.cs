using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menus
{
	public class MessageController : MonoBehaviour 
	{		//displays the status messages on the left side of the screen

		public static MessageController Instance;
		
		[SerializeField]
		private Text _userMessages;
		[SerializeField]
		private Image _bk;
		private string _userMessagesTxt;
	
		private bool 
			_displayMessage, 
			_garbleOn, 		//flag to stop incoming messages during garble processing
			_empOn, 			//under effects of EMP
			_textOverflow;

		private float _displayTimer, _idleTimer, _removeLineTimer;

		private int _addedQueTime;

		private char[] _garbledChars = new char[12]{' ', '_', '*', '?', '.', '^', '~', '-', ',','$','#','&'};

		public int 
			SingleLineTime = 2,			//2 
			MaxQueTime = 20,			//10 messages, then start removing oldest
			IdleTime = 8;				//8

		private SoundFX _soundFx;

		private void Awake()
		{
			Instance = this;
		}

		void Start () 
		{
			_soundFx = GameObject.Find ("SoundFX").GetComponent<SoundFX> ();
		}

		void Update () {
			if(_displayMessage)
			{
				_displayTimer += Time.deltaTime;
				_idleTimer += Time.deltaTime;

				if(_displayTimer>_addedQueTime||_idleTimer>IdleTime)		//idleTimer - we have a large stack of identical messages
				{														//no point waiting 20 seconds
					_displayTimer = 0;
					_idleTimer = 0;
					ResetUserMessage();
				}

				if(_textOverflow)
				{
					_removeLineTimer+=Time.deltaTime;
					if(_removeLineTimer>=1.0f)
					{
						RemoveLines();
						_removeLineTimer = 0;
						_displayTimer = 0;
						_idleTimer = 0;
					}
				}
			}
		}

		public void EndGarble()
		{
			_empOn = false;
		}

		public void GarbleMessage()				//garbles messages after EMP
		{
			//_empOn = true;
			//_garbleOn = true;		//prevent other operations on text during this
			//string[] lines = _userMessagesTxt.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

			//string garbledLines = "";

			//for (int i = 0; i < lines.Length; i++) 
			//{
			//	char[] chars = lines[i].ToCharArray();

			//	List<char> charList = new List<char> (); 
			
			//	for (int j = 0; j < chars.Length; j++) 
			//	{
			//		int garb = UnityEngine.Random.Range (0, 4);
				
			//		if(garb==0)
			//		{
			//			int gIndex = UnityEngine.Random.Range(0,11);
			//			charList.Add(_garbledChars[gIndex]);
			//		}
			//		else
			//		{
			//			charList.Add(chars[j]);
			//		}
			//	}

			//	char[] result = charList.ToArray();

			//	if(i < lines.Length-1)
			//		garbledLines += new string(result)+"\n";
			//	else
			//		garbledLines += new string(result);
			//}

			//_userMessagesTxt = garbledLines;
			//_userMessages.text = _userMessagesTxt;
			//_garbleOn = false;		
		}

		private void CountLines()
		{
			string[] separateLines = _userMessagesTxt.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
			_addedQueTime = 0;

			for (int i = 0; i < separateLines.Length; i++) 
			{
				_addedQueTime += SingleLineTime; 
			}
		}

		public void DisplayMessage(string text)
		{
            //if (_garbleOn) return;  //no new messages during garble

            //if (_soundFx != null)
            //{
            //    _soundFx.Move();
            //}

            //if (!_bk.enabled)
            //{
            //    _bk.enabled = true;
            //}

            //_addedQueTime += SingleLineTime;

            //if (_addedQueTime > MaxQueTime)//in case messages keep coming, start scrolling down
            //{
            //    _textOverflow = true;
            //}

            //_userMessagesTxt += text + "\n";

            //CountLines();
            //_idleTimer = 0;
            //_displayMessage = true;
            //_userMessages.text = _userMessagesTxt;

            //if (_empOn) GarbleMessage();
        }

		private void RemoveLines()
		{
			if(_addedQueTime <= MaxQueTime){ _textOverflow = false; return;}//print ("removing one line. total time: " +addedQueTime.ToString());

			string[] lines = _userMessagesTxt.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);//to preserve the last input //{"\r\n", "\n"}
		
			//int linesToSkip = (int)Math.Abs((addedQueTime - maxQueTime)/singleLineTime);			
			//print (linesToSkip.ToString());
		
			_userMessagesTxt= "";
			_addedQueTime = 0;
			_displayTimer = 0;
		
			/*
		By removing just one line, this thing can create a very unusual glitch, where messages start piling up; 
		the reason is that sometimes, removing one line still allows the queue to grow indefinitely, 
		since this function runs not at update, but with each new message that arrives, at considerably longer intervals
		Possible cause- receiving multiple DisplayMessage requests per update cycle from different sources.
		*/

			//linesToSkip
			for (int i = 1; i < lines.Length-1; i++) //start from 1 - discard oldest message; also discard last empty line
			{
				_addedQueTime += SingleLineTime;
				_userMessagesTxt += lines[i] + "\n" ;
			}
			_userMessages.text = _userMessagesTxt;
		}

		private void ResetUserMessage()
		{
			_soundFx.End ();			//erase curent message queue
			_userMessagesTxt = "";
			_displayMessage = false;
			_addedQueTime = 0; 
			_userMessages.text = _userMessagesTxt;
			_bk.enabled = false;
		}
	}
}
