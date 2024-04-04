using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public Button dealBtn;
	public Button hitBtn;
	public Button standBtn;
	public Button betBtn;

	private int standClicks = 0;

	public PlayerScript playerScript;
	public PlayerScript dealerScript;

	public Text scoreText;
	public Text dealerScoreText;
	public Text betsText;
	public Text cashText;
	public Text mainText;
	public Text standBtnText;

	public GameObject hideCard;
	// How much is bet
	int pot = 0;

	void Start()
	{
		dealBtn.onClick.AddListener(() => DealClicked());
		hitBtn.onClick.AddListener(() => HitClicked());
		standBtn.onClick.AddListener(() => StandClicked());
		betBtn.onClick.AddListener(() => BetClicked());
	}

	private void DealClicked()
	{
		playerScript.ResetHand();
		dealerScript.ResetHand();
		dealerScoreText.gameObject.SetActive(false);
		mainText.gameObject.SetActive(false);
		dealerScoreText.gameObject.SetActive(false);
		GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
		playerScript.StartHand();
		dealerScript.StartHand();
		scoreText.text = "Hand: " + playerScript.handValue.ToString();
		dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();
		hideCard.GetComponent<Renderer>().enabled = true;
		dealBtn.gameObject.SetActive(false);
		hitBtn.gameObject.SetActive(true);
		standBtn.gameObject.SetActive(true);
		standBtnText.text = "Stand";
		pot = 40;
		betsText.text = "Bets: " + pot.ToString() + "₺";
		playerScript.AdjustMoney(-20);
		cashText.text = playerScript.GetMoney().ToString() + "₺";

	}

	private void HitClicked()
	{
		if (playerScript.cardIndex <= 10)
		{
			playerScript.GetCard();
			scoreText.text = "Hand: " + playerScript.handValue.ToString();
			if (playerScript.handValue > 20) RoundOver();
		}
	}

	private void StandClicked()
	{
		standClicks++;
		if (standClicks > 1) RoundOver();
		HitDealer();
		standBtnText.text = "Call";
	}

	private void HitDealer()
	{
		while (dealerScript.handValue < 16 && dealerScript.cardIndex < 10)
		{
			dealerScript.GetCard();
			dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();
			if (dealerScript.handValue > 20) RoundOver();
		}
	}

	void RoundOver()
	{
		bool playerBust = playerScript.handValue > 21;
		bool dealerBust = dealerScript.handValue > 21;
		bool player21 = playerScript.handValue == 21;
		bool dealer21 = dealerScript.handValue == 21;
		if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) return;
		bool roundOver = true;
		if (playerBust && dealerBust)
		{
			mainText.text = "All Bust: Bets returned";
			playerScript.AdjustMoney(pot / 2);
		}
		else if (playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue))
		{
			mainText.text = "Dealer wins!";
		}
		else if (dealerBust || playerScript.handValue > dealerScript.handValue)
		{
			mainText.text = "You win!";
			playerScript.AdjustMoney(pot);
		}
		else if (playerScript.handValue == dealerScript.handValue)
		{
			mainText.text = "Push: Bets returned";
			playerScript.AdjustMoney(pot / 2);
		}
		else
		{
			roundOver = false;
		}
		if (roundOver)
		{
			hitBtn.gameObject.SetActive(false);
			standBtn.gameObject.SetActive(false);
			dealBtn.gameObject.SetActive(true);
			mainText.gameObject.SetActive(true);
			dealerScoreText.gameObject.SetActive(true);
			hideCard.GetComponent<Renderer>().enabled = false;
			cashText.text = playerScript.GetMoney().ToString() + "₺";
			standClicks = 0;
		}
	}

	void BetClicked()
	{
		Text newBet = betBtn.GetComponentInChildren<Text>();
		if (newBet != null)
		{
			string betText = newBet.text;
			string betAmountString = betText.Replace("₺", "").Trim();

			if (int.TryParse(betAmountString, out int intBet))
			{
				playerScript.AdjustMoney(-intBet);
				cashText.text = playerScript.GetMoney().ToString() + "₺";
				pot += (intBet * 2);
				betsText.text = "Bets: " + pot.ToString() + "₺";
			}
			else
			{
				Debug.LogError("Bet amount is not in a correct format: " + betAmountString);
			}
		}
		else
		{
			Debug.LogError("Bet button does not contain a Text component.");
		}
	}
}
