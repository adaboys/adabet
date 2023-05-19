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

use Illuminate\Bus\Queueable;
use Illuminate\Notifications\Notification;
use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Notifications\Messages\MailMessage;
use Illuminate\Support\HtmlString;

class UserMessage extends Notification
{
    use Queueable;

    private $subject;
    private $greeting;
    private $message;

    
    public function __construct($subject, $greeting, $message)
    {
        $this->subject = $subject;
        $this->greeting = $greeting;
        $this->message = $message;
    }

    
    public function via($notifiable)
    {
        return ['mail'];
    }

    
    public function toMail($notifiable)
    {
        return (new MailMessage)
            ->subject($this->subject)
            ->greeting($this->greeting)
            ->line(new HtmlString(nl2br($this->message)));
    }

    
    public function toArray($notifiable)
    {
        return [
            
        ];
    }
}
