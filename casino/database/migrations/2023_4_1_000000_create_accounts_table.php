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

use Illuminate\Support\Facades\Schema;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Database\Migrations\Migration;

class CreateAccountsTable extends Migration
{
    
    public function up()
    {
        Schema::create('accounts', function (Blueprint $table) {
            
            $table->bigIncrements('id');
            $table->unsignedBigInteger('user_id');
            $table->decimal('balance', 20, 2)->default(0);
            $table->timestamps();
            
            $table->foreign('user_id')->references('id')->on('users')->onUpdate('cascade')->onDelete('cascade');
        });
    }

    
    public function down()
    {
        Schema::dropIfExists('accounts');
    }
}
