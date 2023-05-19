<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Facades;

use Illuminate\Support\Facades\Facade;

class AccountTransaction extends Facade
{
    protected static function getFacadeAccessor()
    {
        return 'account_transaction';
    }
}
