using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Timers;
using System;

public class SceneController : MonoBehaviour
{
	public TextMeshProUGUI countDown;
	public TextMeshProUGUI level;
	public TextMeshProUGUI parameterFirst;
	public TextMeshProUGUI _operator;
	public TextMeshProUGUI parameterSecond;
	public TextMeshProUGUI equal;
	public TextMeshProUGUI result;
	public TextMeshProUGUI resultFirst;
	public TextMeshProUGUI resultSecond;
	public TextMeshProUGUI resultThird;
	public GameObject goodbyeCanvas;
	public GameObject mainCanvas;

	AudioSource audioSource;
	public AudioClip yeah;

	const float initIntCountDown = 10;
	private float intCountdown;
	private int intParameterFirst;
	private int intParameterSecond;
	private string stringResult = "?";
	private int intResult;
	private int intResultFirst;
	private int intResultSecond;
	private int intResultThird;


	private int step = 20;
	private int level1;
	private int level2;
	private int level3;
	private int level4;
	private int level5;

	private bool isAllowUpdate = true;
	const int initCountOperator = 15;
	int countOperator = 0;
	int ControLevel = 1;
	float timeForAnimation = 3;


	private void Awake()
	{
		level1 = 20;
		level2 = level1 + step;
		level3 = level2 + step;
		level4 = level3 + step;
		level5 = level4 + step;
		intCountdown = initIntCountDown;


		goodbyeCanvas.SetActive(false);
		mainCanvas.SetActive(true);
	}

	// Start is called before the first frame update
	void Start()
	{
		// set ui:
		newOperator(level5);
		audioSource = GetComponent<AudioSource>();
		level.text = "Level 05";


	}

	// Update is called once per frame
	void Update()
	{
		// set all text:
		parameterFirst.text = intParameterFirst.ToString();
		parameterSecond.text = intParameterSecond.ToString();
		result.text = stringResult;

		resultFirst.text = intResultFirst.ToString();
		resultSecond.text = intResultSecond.ToString();
		resultThird.text = intResultThird.ToString();

		// countDown not update when it relax
		if (isAllowUpdate)
		{
			countDown.text = ((int)intCountdown).ToString();

			// timer:
			intCountdown -= Time.deltaTime;
			if (intCountdown <= 0.0f)
			{
				StartCoroutine(finishFiveSeconds());
				isAllowUpdate = false;
			}
		}
	}



	int[] Shuffle(int[] array)
	{
		// Knuth shuffle algorithm :: courtesy of Wikipedia :)
		for (int t = 0; t < array.Length; t++)
		{
			int tmp = array[t];
			int r = UnityEngine.Random.Range(t, array.Length);
			array[t] = array[r];
			array[r] = tmp;
		}
		return array;
	}

	int GetRandom(int level)
	{
		return UnityEngine.Random.Range(level - step, level);
	}

	int GetRandomForSecondParameter(int level)
	{
		return UnityEngine.Random.Range(0, level);
	}

	int GetRandomResultExcept(int level, int[] excepts)
	{
		var rand = UnityEngine.Random.Range((level - step) + 0, level + level);
		for (int i = 0; i < excepts.Length; i++)
		{
			if (rand == excepts[i])
			{
				return GetRandomResultExcept(level, excepts);
			}
		}

		return rand;
	}


	void newOperator(int level)
	{
		// reset count:
		intCountdown = initIntCountDown;

		// first
		intParameterFirst = GetRandom(level);

		//second
		intParameterSecond = GetRandomForSecondParameter(level);

		//stringResult:
		stringResult = "?";

		//result 
		intResult = intParameterFirst + intParameterSecond;

		//result1
		var intResult1 = GetRandomResultExcept(level, new int[] { intResult });

		//result2:
		var intResult2 = GetRandomResultExcept(level, new int[] { intResult, intResult1 });

		var shuffledArray = Shuffle(new int[] { intResult, intResult1, intResult2 });
		intResultFirst = shuffledArray[0];
		intResultSecond = shuffledArray[1];
		intResultThird = shuffledArray[2];

		//increment countOperator:
		countOperator++;
	}

	private IEnumerator finishFiveSeconds()
	{
		stringResult = intResult.ToString();
		audioSource.PlayOneShot(yeah, 0.5f);

		//hightlight answer correct:
		HightLight();

		yield return new WaitForSeconds(timeForAnimation);

		//check each level is 15 time operator:
		if (countOperator >= initCountOperator)
		{
			// show popup:
			goodbyeCanvas.SetActive(true);
			mainCanvas.SetActive(false);
		}
		else
		{
			newOperator(level5);
			isAllowUpdate = true;
		}
	}

	void HightLight()
	{
		int _intResultFirst = Convert.ToInt32(resultFirst.text);
		int _intResultSecond = Convert.ToInt32(resultSecond.text);

		if (_intResultFirst == intResult)
		{
			StartCoroutine(startTextMeshAnimation(resultFirst, () => {
				keepAnimating = false;
			}));
		}
		else if (_intResultSecond == intResult)
		{
			StartCoroutine(startTextMeshAnimation(resultSecond, () => {
				keepAnimating = false;
			}));
		}
		else
		{
			StartCoroutine(startTextMeshAnimation(resultThird, () => {
				keepAnimating = false;
			}));
		}
	}








	//public TextMesh textMesh;
	public float animSpeedInSec = 1f;
	bool keepAnimating = false;

	private IEnumerator anim(TextMeshProUGUI textMesh)
	{

		Color currentColor = textMesh.color;

		Color invisiblecolor = Color.red;
		//invisibleColor.a = 0; //Set Alpha to 0

		float oldAnimSpeedInSec = animSpeedInSec;
		float counter = 0;

		float delta = 0;


		while (keepAnimating) {

			//Hide Slowly
			while (counter < oldAnimSpeedInSec)
			{
				if (!keepAnimating)
				{
					yield break;
				}

				//Reset counter when Speed is changed
				if (oldAnimSpeedInSec != animSpeedInSec)
				{
					counter = 0;
					oldAnimSpeedInSec = animSpeedInSec;
				}

				counter += Time.deltaTime;
				textMesh.color = Color.Lerp(currentColor, invisiblecolor, counter / oldAnimSpeedInSec);
				yield return null;
			}

			yield return null;


			//Show Slowly
			while (counter > 0)
			{
				if (!keepAnimating)
				{
					yield break;
				}

				//Reset counter when Speed is changed
				if (oldAnimSpeedInSec != animSpeedInSec)
				{
					counter = 0;
					oldAnimSpeedInSec = animSpeedInSec;
				}

				counter -= Time.deltaTime;
				textMesh.color = Color.Lerp(currentColor, invisiblecolor, counter / oldAnimSpeedInSec);
				yield return null;
			}
		}

		//textMesh.color = currentColor;
	}

	//Call to Start animation
	IEnumerator startTextMeshAnimation(TextMeshProUGUI textMesh, Action action)
	{
		//if (keepAnimating)
		//{
		//	return;
		//}
		var currentColor = textMesh.color;

		keepAnimating = true;
		StartCoroutine(anim(textMesh));
		yield return new WaitForSeconds(timeForAnimation);
		action();
		textMesh.color = currentColor;
	}

	//Call to Change animation Speed
	void changeTextMeshAnimationSpeed(float animSpeed)
	{
		animSpeedInSec = animSpeed;
	}



}
