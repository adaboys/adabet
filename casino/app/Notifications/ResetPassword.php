<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Notifications;

use Illuminate\Notifications\Messages\MailMessage;
use Illuminate\Auth\Notifications\ResetPassword as Notification;

class ResetPassword extends Notification
{
    
    public function toMail($notifiable)
    {
        return (new MailMessage)
            ->subject(__('Password reset'))
            ->line(__('You are receiving this email because we received a password reset request for your account.'))
            ->action(__('Reset password'), sprintf('%s/password/reset/%s?email=%s', request()->getSchemeAndHttpHost(), $this->token, urlencode($notifiable->email)))
            ->line(__('If you did not request a password reset, no further action is required.'));
    }
}
