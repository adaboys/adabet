<?php
/**
 *   Stake iGaming platform
 *   ----------------------
 *   EncryptCookies.php
 * 
 *   @copyright  Copyright (c) FinancialPlugins, All rights reserved
 *   @author     FinancialPlugins <info@financialplugins.com>
 *   @see        https://financialplugins.com
*/

namespace App\Http\Middleware;

use Closure;
use Illuminate\Cookie\Middleware\EncryptCookies as Middleware;

class EncryptCookies extends Middleware
{
    
    protected $except = [
        
    ];

    public function handle($r, Closure $next)
    {
        return parent::handle($r, $next);
    }
}
