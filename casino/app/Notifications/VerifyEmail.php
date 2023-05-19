<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   verifyEmail.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Notifications;

use Illuminate\Auth\Notifications\VerifyEmail as Notification;

class VerifyEmail extends Notification
{
    
    protected function verificationUrl($notifiable)
    {
        $url = parent::verificationUrl($notifiable);

        return str_replace('/api', '', $url);
    }
}
