<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Services\Crypto;

interface Signer
{
    public function sign(string $privateKey, string $message): string;

    public function verify(string $message, string $signature, string $address): bool;
}
