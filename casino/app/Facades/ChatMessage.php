<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   ChatMessage.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Facades;

use Illuminate\Support\Facades\Facade;

class ChatMessage extends Facade
{
    protected static function getFacadeAccessor()
    {
        return 'chat_message';
    }
}
