<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   database/migrations
 * 
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
*/

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateCacheLocksTable extends Migration
{
    
    public function up()
    {
        Schema::create('cache_locks', function (Blueprint $table) {
            $table->string('key')->primary();
            $table->string('owner');
            $table->unsignedInteger('expiration');

            $table->index('owner');
            $table->index('expiration');
        });
    }

    
    public function down()
    {
        Schema::dropIfExists('cache_locks');
    }
}
