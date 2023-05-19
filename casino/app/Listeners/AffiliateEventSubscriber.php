<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Listeners;

use App\Events\GamePlayed;
use App\Models\AffiliateCommission;
use App\Services\AffiliateService;
use Illuminate\Auth\Events\Registered;
use Packages\Raffle\Events\RaffleTicketBought;

class AffiliateEventSubscriber
{
    public function generateUserSignupCommission(Registered $event)
    {
        $user = $event->user;

        $affiliateService = new AffiliateService($user->account);

        $affiliateService->createCommissions(
            $user,
            AffiliateCommission::TYPE_SIGN_UP
        );
    }

    public function generateGameCommission(GamePlayed $event)
    {
        $game = $event->game;

        $affiliateService = new AffiliateService($game->account);

        $loss = max(0, $game->bet - $game->win); 
        $win = max(0, $game->win - $game->bet); 

        if ($loss > 0) {
            $affiliateService->createCommissions(
                $event->game,
                AffiliateCommission::TYPE_GAME_LOSS
            );
        }

        if ($win > 0) {
            $affiliateService->createCommissions(
                $event->game,
                AffiliateCommission::TYPE_GAME_WIN
            );
        }
    }

    public function generateRaffleTicketCommission(RaffleTicketBought $event)
    {
        $affiliateService = new AffiliateService($event->user->account);

        $affiliateService->createCommissions(
            $event->raffleTicket,
            AffiliateCommission::TYPE_RAFFLE_TICKET
        );
    }

    
    public function subscribe($events)
    {
        $events->listen(
            Registered::class,
            [self::class, 'generateUserSignupCommission']
        );

        $events->listen(
            GamePlayed::class,
            [self::class, 'generateGameCommission']
        );

        $events->listen(
            RaffleTicketBought::class,
            [self::class, 'generateRaffleTicketCommission']
        );
    }
}
