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
use App\Notifications\Admin\GameLoss;
use App\Notifications\Admin\GameWin;
use App\Notifications\Admin\NewUser;
use Illuminate\Auth\Events\Registered;
use Illuminate\Contracts\Auth\MustVerifyEmail;
use Illuminate\Support\Facades\Notification;

class NotificationSubscriber
{
    private $adminEmail;

    public function __construct()
    {
        $this->adminEmail = config('settings.notifications.admin.email');
    }

    public function sendEmailVerificationLink(Registered $event)
    {
        if ($event->user instanceof MustVerifyEmail && ! $event->user->hasVerifiedEmail()) {
            $event->user->sendEmailVerificationNotification();
        }
    }

    public function sendNewUserAdminNotification(Registered $event)
    {
        if (!$event->user->is_bot) {
            Notification::route('mail', $this->adminEmail)
                ->notify(new NewUser($event->user));
        }
    }

    public function sendGameWinAdminNotification(GamePlayed $event)
    {
        if ($event->game->win >= config('settings.notifications.admin.game.win.treshold')) {
            Notification::route('mail', $this->adminEmail)
                ->notify(new GameWin($event->game));
        }
    }

    public function sendGameLossAdminNotification(GamePlayed $event)
    {
        if ($event->game->win < $event->game->bet && abs($event->game->profit) >= config('settings.notifications.admin.game.loss.treshold')) {
            Notification::route('mail', $this->adminEmail)
                ->notify(new GameLoss($event->game));
        }
    }

    
    public function subscribe($events)
    {
        
        if (config('settings.users.email_verification')) {
            $events->listen(Registered::class, self::class . '@sendEmailVerificationLink');
        }

        
        if ($this->adminEmail) {
            if (config('settings.notifications.admin.registration.enabled')) {
                $events->listen(Registered::class, self::class . '@sendNewUserAdminNotification');
            }

            if (config('settings.notifications.admin.game.win.enabled')) {
                $events->listen(GamePlayed::class, self::class . '@sendGameWinAdminNotification');
            }

            if (config('settings.notifications.admin.game.loss.enabled')) {
                $events->listen(GamePlayed::class, self::class . '@sendGameLossAdminNotification');
            }
        }
    }
}
