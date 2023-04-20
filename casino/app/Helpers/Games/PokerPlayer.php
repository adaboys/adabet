<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   PockerPlayer.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */


namespace App\Helpers\Games;

class PokerPlayer
{
    protected $pocketCards;
    protected $pokerHandClass = PokerHand::class;

    protected $hand;

    public function __construct()
    {
        $this->pocketCards = new CardSet();

        return $this;
    }

    public function getPocketCards(): CardSet
    {
        return $this->pocketCards;
    }

    public function addPocketCard(Card $card): PokerPlayer
    {
        $this->pocketCards->push($card);

        return $this;
    }

    public function addPocketCards(CardSet $cards): PokerPlayer
    {
        $this->pocketCards->push(...$cards);

        return $this;
    }

    public function getHand(): PokerHand
    {
        return $this->hand;
    }

    public function makeHand(CardSet $communityCards): PokerPlayer
    {
        $this->hand = new $this->pokerHandClass($this->getPocketCards(), $communityCards);

        return $this;
    }
}
