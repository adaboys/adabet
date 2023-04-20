<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */
namespace App\Repositories;

use App\Models\Account;
use App\Models\User;

class AccountRepository
{
    
    public static function create(User $user): Account
    {
        $account = new Account();
        $account->user()->associate($user);
        $account->save();

        return $account;
    }
}
