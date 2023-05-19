<?php
/**
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */
namespace App\Rules;

use App\Models\User;
use Illuminate\Contracts\Validation\Rule;

class BalanceIsSufficient implements Rule
{
    private $user;
    private $requiredAmount;

    
    public function __construct(User $user, $requiredAmount)
    {
        $this->user = $user;
        $this->requiredAmount = floatval($requiredAmount);
    }

    
    public function passes($attribute = NULL, $value = NULL)
    {
        return $this->user->account->balance >= $this->requiredAmount;
    }

    
    public function message()
    {
        return __('Insufficient balance to perform this operation.');
    }
}
